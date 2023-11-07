/**
 * Indexing Script Template
 *
 * You may either leave the methods in this template empty or remove them,
 * if you don't want to handle the specific events.
 *
 * The fieldFocusGained, fieldFocusLost, fieldChanged, fieldOcrCompleted, 
 * fieldLoad, fieldSave methods are called for
 * every field. To write specific code for a field, use it's name:
 *   if (field.name == 'FieldName') {
 *     //code here specific to field with name 'FieldName'
 *   }
 * Alternatively, you can write specific methods to handle field specific events
 * by writing methods with names <FieldName>FocusGained, <Fieldname>FocusLost,
 * <FieldName>Changed, <FieldName>OcrCompleted, <FieldName>Load, <FieldName>Save. 
 * 
 * For example, for a field with name 'Address', you can write the following specific methods:
 *  AddressFocusGained, AddressFocusLost, AddressChanged, etc...
 *  
 * Notice that if the field name contains any characters other than letters,
 * digits or underscore (_), they are replaced with an underscore. So, a field
 * whose name is "First Name" will be mapped to functions with names:
 *  First_NameFocusGained, First_NameFocusLost, First_NameChanged
 * In the case that there is a specific function for a field, the generic
 * method is not called at all for this field.
 * The field methods support special return values: see the comments before each
 * method for details.
 *
 * The standard output and error streams can be accessed thourgh the variables
 * 'out' and 'err'. For example:
 *   out.println("Hello World");
 *   err.println("This goes to standard error");
 * Normally, standard out and err are redirected to the client log file.
 * You may use the 'debug' object to direct output to the 'Debug output' window
 *
 * The following global objects are always defined:
 *   batch      (see Javadoc for com.imagetrust.tc.model.object.IBatch)
 *   job        (see Javadoc for com.imagetrust.tc.model.object.IJob)
 *   MainWindow (see Javadoc for java.awt.Window)
 *   IndexPane  (see Javadoc for com.imagetrust.tc.model.object.IIndexingContext)
 *   Engine     (see Javadoc for com.imagetrust.tc.model.object.IScript)
 *   ITUtils
 *   loggedUser (see Javadoc for com.imagetrust.tc.model.object.IUser)
 *
 *  After the preProcess method is called, these objects are also defined
 *  based on the context:
 *   document (if this script is for a Document class)
 *   folder (if this script is for a Folder class)
 *   node (always, identical to document or folder)
 *
 *  You can access the current list of fields from any method (except the
 *  load and unload) using the 'fields' property of the 'node' object.
 *  For example:
 *    node.fields['Address'].value = 'An address';
 *    node.fields['Company Name'].visible = false;
 *
 *  ----------------------------------------------------------------------------
 *  
 *  Scripting evaluation
 *  --------------------
 *  Since scripts are attached to Document and Folder classes, they are evaluated
 *  once every time Indexing starts and when the script needs to load.
 *  It is important to remember the evaluation context to determine the scope of
 *  global variables and functions. The load() method is called exactly after
 *  the evaluation of the script.
 *  
 *  In Test/Debug mode, evaluation of the script takes place when the script is
 *  updated.
 *  
 */

var dbServer = new DbServer('MAF BDD');

/**
 * Called the first time a document of this class is loaded.
 * This method is called every time the client gets into Indexing mode (not 
 * when Indexing starts, but when a script is loaded, as needed).
 */
function load(batch) {

}


/**
 * Called when ending indexing operation.
 * This method is called every time the client exits the Indexing mode.
 * As a result, this method is also called after a Batch is closed/suspended, 
 * when the client removes the Batch and exits the Indexing mode. In this
 * case, any changes you make to the Batch from the unload method
 * will not have any effect.
 */
function unload(batch) {
}

/**
 * Called before starting indexing on a node of this class.
 * Supported return values:
 *   Const.MoveToNextNode: index next node
 *   { MoveToField: '<FieldName>' }: set focus to specific field
 */
function preProcess(node) {
  var requete = "select distinct(TYPE_DOC) from C_CHARTEDOC as c INNER JOIN PARAM_COTE as vpc on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' order by TYPE_DOC";
  var query = dbServer.query(requete);
  fill(node.fields['TYPE_DE_DOCUMENT'], query);

  requete = "SELECT vpc.CODE_ORIGINE, LIB_ORIGINE FROM PARAM_COTE vpc inner join [REF_ORIGINE ] vro on vro.CODE_ORIGINE = vpc.CODE_ORIGINE inner join dbo.C_CHARTEDOC cc on vpc.CODE_TYPE_DOC = cc.CODE_TYPE_DOC WHERE cc.ACTIVITE LIKE '%PAPS%'";
  var query = dbServer.query(requete);
  fill(node.fields['ORIGINE_DU_DOCUMENT'], query);

  var requete2 = "SELECT DISTINCT LIB_COTE FROM REF_COTE";
  var query2 = dbServer.query(requete2);
  //fill(node.fields['COTE_DU_DOCUMENT'], query2);

}

/**
 * Called after finishing indexing on a node of this class
 */
function postProcess(node) {

  var requete = "SELECT [Code_Redacteur] FROM [GEDMAF].[dbo].[V_ALL_UTILISATEUR] where [compte_nt] ='" + node.fields['Operateur'].value + "'";
  var query = dbServer.query(requete);
  //alert(node.fields['Operateur'].value);
  //alert(query);
  if (query.length == 1) {
    node.fields['Trigramme'].value = query[0][0];
  }

  if (node.fields['TYPE_DE_DOCUMENT'].value != '' && node.fields['COTE_DU_DOCUMENT'].value != '' && node.fields['LIBELLE_DOCUMENT'].value == '') {
    node.fields['LIBELLE_DOCUMENT'].value = node.fields['COTE_DU_DOCUMENT'].value + " - " + node.fields['TYPE_DE_DOCUMENT'].value;
  }

}

/**
 * Called when a field gains focus. 
 * For multivalued fields <code>index</code> is the index of the value loosing the focus, for normal fields it is always 0.
 * Supported return values:
 *  Const.MoveToNextField: set focus to next field
 *  Const.MoveToNextNode: index next node
 *  { MoveToField: '<FieldName>' }: set focus to specific field
 */
function fieldFocusGained(field, index) {

}

/**
 * Called before a field loses focus. 
 * For multivalued fields <code>index</code> is the index of the value loosing the focus, for normal fields it is always 0.
 * Return 'false' to cancel focus transfer.
 * You may use the markValid() and markInvalid() methods to mark whether the
 * value of the field is valid or not.
 * Supported return values:
 *  true: allow focus to move to next field
 *  false: forces focus to remain to this field
 *  Const.MoveToNextNode: index next node
 */
function fieldFocusLost(field, index) {
  if (field.name == 'NO_DOSSIER_SINISTRE' && field.value != '') {
    var requete = "select CONCAT(NO_DOSSIER,'||',IsNull(GROUPE, '-1'),'||',Nom_redacteur,'||',PreNom_redacteur,'||',numero_Adherent,'||',Code_Redacteur) AS NODOSSIER from [GEDMAF].[dbo].[V_AFFAIRE_PAPS] where NO_DOSSIER LIKE '%" + field.value + "%' order by Code_societe asc";
    var query = dbServer.query(requete);

    if (query.length > 1) {
      node.fields['LISTE_DOSSIER_SINISTRE'].value = '';
      node.fields['NO_ADHERENT'].value = '';
      node.fields['CODE_DU_GESTIONNAIRE'].value = '';
      node.fields['GEST_REDAC'].value = '';
      node.fields['GROUPE'].value = '';
      fill(node.fields['LISTE_DOSSIER_SINISTRE'], query);
    }
    if (query.length == 1) {
      node.fields['LISTE_DOSSIER_SINISTRE'].value = query[0][0];
      var fieldChoice = query[0][0].split("||");
      node.fields['NO_ADHERENT'].value = fieldChoice[4];
      node.fields['CODE_DU_GESTIONNAIRE'].value = fieldChoice[5];
      node.fields['GEST_REDAC'].value = fieldChoice[2] + ' ' + fieldChoice[3];
      node.fields['GROUPE'].value = fieldChoice[1];
    }

    var requete = "SELECT [Code_Redacteur] FROM [GEDMAF].[dbo].[V_ALL_UTILISATEUR] where [compte_nt] ='" + node.fields['Operateur'].value + "'";
    var query = dbServer.query(requete);
    //alert(node.fields['Operateur'].value);
    //alert(query);
    if (query.length == 1) {
      node.fields['Trigramme'].value = query[0][0];
    }

  }

  if (field.name == 'TYPE_DE_DOCUMENT' && field.value != '' && node.fields['ORIGINE_DU_DOCUMENT'].value != '') {
    node.fields['COTE_DU_DOCUMENT'].clearOptions();
    var requete = " SELECT DISTINCT vrc.LIB_COTE FROM REF_COTE as vrc  INNER JOIN PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE inner JOIN dbo.C_CHARTEDOC as c on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC  WHERE c.TYPE_DOC = '" + field.value + "' and vpc.CODE_ORIGINE = '" + node.fields['ORIGINE_DU_DOCUMENT'].value + "'";
    var query = dbServer.query(requete);
    if (query.length > 0) {
      fill(node.fields['COTE_DU_DOCUMENT'], query);
    }
  }
  if (field.name == 'ORIGINE_DU_DOCUMENT' && field.value != '' && node.fields['TYPE_DE_DOCUMENT'].value != '') {
    node.fields['COTE_DU_DOCUMENT'].clearOptions();
    var requete = " SELECT DISTINCT vrc.LIB_COTE FROM REF_COTE as vrc  INNER JOIN PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE inner JOIN dbo.C_CHARTEDOC as c on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC  WHERE c.TYPE_DOC = '" + node.fields['TYPE_DE_DOCUMENT'].value + "' and vpc.CODE_ORIGINE = '" + field.value + "'";
    var query = dbServer.query(requete);

    if (query.length > 0) {
      fill(node.fields['COTE_DU_DOCUMENT'], query);
    }
  }


  return true;
}

/**
 * Called every time the value of a field is changed.
 * You may use the markValid() and markInvalid() methods to mark whether the
 * value of the field is valid or not.
 * 
 * For multivalued fields <code>index</code> is the index of the value loosing the focus, for normal fields it is always 0.
 */
function fieldChanged(field, index) {
  if (field.name == 'LISTE_DOSSIER_SINISTRE') {
    var fieldChoice = field.value.split("||");
    node.fields['NO_ADHERENT'].value = fieldChoice[4];
    node.fields['CODE_DU_GESTIONNAIRE'].value = fieldChoice[5];
    node.fields['GEST_REDAC'].value = fieldChoice[2] + ' ' + fieldChoice[3];
    node.fields['GROUPE'].value = fieldChoice[1];
  }

  if (field.name == 'TYPE_DE_DOCUMENT') {
    if (node.fields['TYPE_DE_DOCUMENT'].value == '') {
      node.fields['ORIGINE_DU_DOCUMENT'].value = '';

      var requete = "select distinct(TYPE_DOC) from C_CHARTEDOC as c INNER JOIN PARAM_COTE as vpc on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' order by TYPE_DOC";
      var query = dbServer.query(requete);
      fill(node.fields['TYPE_DE_DOCUMENT'], query);


    }
    var requete = "SELECT vpc.CODE_ORIGINE, LIB_ORIGINE FROM PARAM_COTE vpc inner join [REF_ORIGINE ] vro on vro.CODE_ORIGINE = vpc.CODE_ORIGINE inner join dbo.C_CHARTEDOC cc on vpc.CODE_TYPE_DOC = cc.CODE_TYPE_DOC WHERE  cc.TYPE_DOC = '" + node.fields['TYPE_DE_DOCUMENT'].value + "' AND cc.ACTIVITE LIKE '%PAPS%'";
    var query = dbServer.query(requete);
    fill(node.fields['ORIGINE_DU_DOCUMENT'], query);


    node.fields['COTE_DU_DOCUMENT'].value = '';
    var requete = " SELECT DISTINCT vrc.LIB_COTE FROM REF_COTE as vrc  INNER JOIN PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE inner JOIN dbo.C_CHARTEDOC as c on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC  WHERE c.TYPE_DOC = '" + field.value + "' and vpc.CODE_ORIGINE = '" + node.fields['ORIGINE_DU_DOCUMENT'].value + "'";
    var query = dbServer.query(requete);
    if (query.length > 0) {
      fill(node.fields['COTE_DU_DOCUMENT'], query);
    }

  }

  if (field.name == 'ORIGINE_DU_DOCUMENT') {
    //node.fields['TYPE_DE_DOCUMENT'].value='';
    var requete = "select distinct(TYPE_DOC) from C_CHARTEDOC as c INNER JOIN PARAM_COTE as vpc on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC where FILTRE = '1' AND ACTIVITE LIKE '%PAPS%' AND vpc.CODE_ORIGINE = '" + node.fields['ORIGINE_DU_DOCUMENT'].value + "' order by TYPE_DOC";
    var query = dbServer.query(requete);
    fill(node.fields['TYPE_DE_DOCUMENT'], query);

    node.fields['COTE_DU_DOCUMENT'].value = '';
    var requete = " SELECT DISTINCT vrc.LIB_COTE FROM REF_COTE as vrc  INNER JOIN PARAM_COTE as vpc on vrc.CODE_COTE = vpc.CODE_COTE inner JOIN dbo.C_CHARTEDOC as c on c.CODE_TYPE_DOC = vpc.CODE_TYPE_DOC  WHERE c.TYPE_DOC = '" + field.value + "' and vpc.CODE_ORIGINE = '" + node.fields['ORIGINE_DU_DOCUMENT'].value + "'";
    var query = dbServer.query(requete);
    if (query.length > 0) {
      fill(node.fields['COTE_DU_DOCUMENT'], query);
    }
  }
}


/**
 * 
 * Called whenever the structure of a table is changed through the indexing UI (e.g. inserting/deleting rows).
 * 
 */
function tableChanged(table) {
}

/**
 * Called for each field of this IndexClass before indexing starts.
 * This method will only be called if you have set-up the 'Load method' to
 * be 'Script' (this is done in the 'Indexing Step Configuration' for an 
 * Index step in a workflow).
 * 
 * This is the method that you should use to 'load' field values from the node properties
 * to the runtime properties of each field of the node being indexed. The following
 * code emulates the 'internal' implementation, which just copies values from the 'idx'
 * namespace to the field values:
 *  function fieldLoad(field){
 *    function get(n){ return node.properties["idx." + field.name + "." + n]; }  
 *    field.value = get('value');
 *    field.markedInvalid = get('markedInvalid') == 'true';
 *    field.review = get('review') == 'true';
 *  }
 * 
 * Usually, it makes sense to override this method in the 'Indexing Step Configuration'
 * script, taking into account the configuration context.
 * 
 * Normally, the properties 'value', 'markedInvalid', 'review' are set.
 */
function fieldLoad(field) {
}

/**
 * Called for each field of this IndexClass after indexing ends.
 * This method will only be called if you have set-up the 'Save method' to
 * be 'Script' (this is done in the 'Indexing Step Configuration' for an 
 * Index step in a workflow).
 * 
 * This is the method that you should use to 'save' runtime field values to 
 * the node's properties for each field of the node being indexed.
 * The following code emulates the 'internal' implementation, which just saves
 * the current field values to the 'idx' namespace:
 *   function fieldSave(field){
 *     function set(n,v){ 
 *       node.properties["idx." + field.name + "." + n] = typeof(v)=='boolean' ? (v?'true':null) : v; 
 *     }
 *     set('value', field.value);
 *     set('markedInvalid', field.markedInvalid);
 *     set('review', field.review);
 *     if (field.isList()){
 *       set('listValue', field.valueAsListItem.value);
 *       set('listDescr', field.valueAsListItem.description);
 *     }
 *   }
 *  
 * Usually, it makes sense to override this method in the 'Indexing Step Configuration'
 * script, taking into account the configuration context.
 * 
 * Normally, the properties 'value', 'markedInvalid', 'review' are set.
 */
function fieldSave(field) {
}

/**
 * This is a function that is called under very specific conditions, and always
 * in combination with the fieldSave(field) method.
 * Specifically, it is called when in an index step configuration, the 'Save method' 
 * for one (or more) fields is set to 'Script'. In those cases, it is not really possible
 * to know the actual namespace in the node's properties that a field's value is saved (since
 * the fieldSave(field) method above is free to use whatever namespace it wants).
 * 
 * If the value of the field needs to be read (in the context of an Index step), then
 * this method will be used to get the namespace to use from the node's properties.
 * If this method is not implemented or does not return a string, then the default
 * namespace 'idx' will be used. The main function that requires reading the values
 * of fields is to decide whether indexing is complete for a given index step.
 * 
 * Since this method may also be called in the server, you should make no assumptions
 * for the JavaScript scope and global objects. To be 100% safe, the implementation
 * of this method should not access any other objects other than the ones given
 * as parameters. Also, the objects in the parameters are given in a read-only context, 
 * so you should not attempt to modify any of their properties.
 * 
 * It usually makes more sense to override this method in the field in the index step
 * conf script, to make it specific to that step (although the actual step is passed
 * as the first parameter).
 * 
 * This method should return a single string, which is the namespace in the node's
 * properties for the given fields. For example:
 *   return 'index2.' + field.name;
 */
function getFieldNamespace(step, node, field) {
}

/**
 * Called whenever the client OCR engine is queried to auto-fill an index field.
 * This method is called only on a Document Class script, because only the 
 * index fields of a Document Class can have an associated Index Zone 
 * on which OCR is performed automatically.
 * This method is called in two different cases:
 *   1. after the fieldLoad() method (if any), during initialization of the index 
 *      pane for a document, if the field is empty
 *   2. Whenever another event (like an image rotation) causes the OCR engine to 
 *      re-run.
 *
 * Do not count on this method to be called each time: the OCR engine or other internal logic
 * may decide that OCR of a field is not necessary or that cached results are enough. 
 * Instead, think of this method as a way to modify the behavior of auto-filling a field
 * with the OCR results.
 * 
 * This method is called immediately after the OCR extraction results become available
 * and before the default logic is executed (the default logic is to use the extracted
 * value with the maximum confidence and set the field value, if the field does not already
 * have a value; the field.review is set if the extraction confidence is less than
 * the minimum confidence level defined for this index zone).
 * 
 * You can arbitrarily modify the properties of the given field (e.g. value, text, review, etc)
 * to override the default behavior: in that case, the method should return 'false' to
 * suppress the default behavior.
 * 
 * Let's say that you want to prepend a specific string (e.g. 'OCR: ') to the
 * extracted value and reset the review flag in all cases. The following code
 * will do that:
 *   if (maxConfidenceData != null){
 *    field.text = 'OCR:' + maxConfidenceData.value;
 *    field.review = false;
 *    return false;
 *   }
 * 
 * Parameters:
 *   field : the index field for which the ocr was performed.
 *   extractionData : a list of all the extracted data (ExtractionData objects), 
 *     from all index zones that refer to the given index field. 
 *     The ExtractionData object has the following properties:
 *       rect, value, confidence, indexZone, page.
 *   maxConfidenceExtractionData: the extracted data (Extraction object) with the 
 *     highest confidence. If the index field uses only one index zone, then the 
 *     maxConfidenceData will be equal to the only item in the extractionData list.
 * Supported return values:
 *  true: if you want the default behavior to be executed (default return value)
 *  false: if you do not want the default behavior to be executed
 */
function fieldOcrCompleted(field, extractionData, maxConfidenceData) {
}

/**
 * This method is called every time a key is pressed and the focus
 * is anywhere in the IndexingPanel. The method is only called for non-printable
 * keys, e.g. for shortcut/action keys (e.g. F1, CTRL+A, ALT+F3, Home, etc).
 * The evt parameter is of type java.awt.event.KeyEvent. The event is called 
 * when the key is releasedand before the default processing of the key.
 * 
 * You can use this method to install arbitrary keyboard shortcuts. 
 * The following example emulates a click on the 'Next' button whenever
 * the user presses 'CTRL+F1'. If the user pressed the F2 key, then 
 * the focus jumps to the 'Address' field.
 *   ...
 *   if (isJavaEngine()) {
 *     importPackage(java.awt.event); //if not already imported
 *     ...
 *     var ks = KeyStroke.getKeyStrokeForEvent(evt);
 *     var isCtrlDown = (ks.getModifiers() & KeyEvent.CTRL_DOWN_MASK) != 0;
 *     var isF1 = ks.getKeyCode() == KeyEvent.VK_F1;
 *     var isF2 = ks.getKeyCode() == KeyEvent.VK_F2;
 *     if (isF1 && isCtrlDown){
 *       IndexPane.next();
 *     }
 *     else if (isF2){
 *       var f = node.fields['Address'];
 *       f.backColor = Color.yellow;
 *       IndexPane.getComponentInfo(f).textField.requestFocusInWindow();
 *     }
 *   }
 *   ...
 *   
 * Supported return values:
 *   true: if you 'consume' the key event and you don't want to be propagated
 *         to the focused control
 *   false: if you want the key event to be propagated to the focused control
 *         (this is the default value, e.g. if you don't return something).
 */
function keyEvent(evt) {
}


function fill(field, rows) {
  field.clearOptions();
  for (i in rows) {
    field.addOption(rows[i][0], rows[i][0]);
  }

  function startsWith(str, reg, offset) {
    var regex = '^.{${0,offset}}(${reg})'
    var final = new RegExp(regex, 'i')
    var found = str.match(final)
    return (found ? true : false)
  }
}