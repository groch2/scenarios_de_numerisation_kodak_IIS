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

/**
 * Called the first time a document of this class is loaded.
 * This method is called every time the client gets into Indexing mode (not 
 * when Indexing starts, but when a script is loaded, as needed).
 */
function load(batch) { }

/**
 * Called when ending indexing operation.
 * This method is called every time the client exits the Indexing mode.
 * As a result, this method is also called after a Batch is closed/suspended, 
 * when the client removes the Batch and exits the Indexing mode. In this
 * case, any changes you make to the Batch from the unload method
 * will not have any effect.
 */
function unload(batch) { }

/**
 * Called before starting indexing on a node of this class.
 * Supported return values:
 *   Const.MoveToNextNode: index next node
 *   { MoveToField: '<FieldName>' }: set focus to specific field
 */
function preProcess(node) {
  document.setProperty(isIndexationValid, false);
}

/**
 * Called after finishing indexing on a node of this class
 */
function postProcess(node) {
  debug.print("postProcess DEBUT");
  const _isIndexationValid = document.getProperty(isIndexationValid);
  if (!_isIndexationValid) {
    return;
  }
  debug.print("postProcess 1");
  const jsonDocumentMetadata = JSON.parse(node.getProperty("jsonDocumentMetadata"));
  const dateNow = new Date();
  jsonDocumentMetadata.dateDocument =
    jsonDocumentMetadata.dateNumerisation =
    jsonDocumentMetadata.deposeLe = dateNow.toJSON();
  const codeUtilisateur = (function () {
    const userName = loggedUser.getUsername();
    const codeUtilisateur =
      get_utilisateur_BUT_from_MAF_BUT_API({ mafDomainUserLogin: userName }).codeUtilisateur.trim();
    return codeUtilisateur;
  })();
  jsonDocumentMetadata.deposePar = codeUtilisateur;
  jsonDocumentMetadata.categoriesFamille = "DOCUMENTS PAPS";
  jsonDocumentMetadata.categoriesCote = "GESTION";
  jsonDocumentMetadata.categoriesTypeDocument = "AR POSTE";
  jsonDocumentMetadata.libelle = "AR POSTE LA POSTE";
  jsonDocumentMetadata.statut = "NOUVEAU";
  jsonDocumentMetadata.canalId = "12";
  jsonDocumentMetadata.FichierNom = "AR_" + formatDateFr(dateNow, "-") + ".pdf";
  node.setProperty("jsonDocumentMetadata", JSON.stringify(jsonDocumentMetadata));
  debug.print("postProcess FIN");
}

/**
 * Called when a field gains focus. 
 * For multivalued fields <code>index</code> is the index of the value loosing the focus, for normal fields it is always 0.
 * Supported return values:
 *  Const.MoveToNextField: set focus to next field
 *  Const.MoveToNextNode: index next node
 *  { MoveToField: '<FieldName>' }: set focus to specific field
 */
function fieldFocusGained(field, index) { }

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
function fieldFocusLost(field, index) { return true; }

/**
 * Called every time the value of a field is changed.
 * You may use the markValid() and markInvalid() methods to mark whether the
 * value of the field is valid or not.
 * 
 * For multivalued fields <code>index</code> is the index of the value loosing the focus, for normal fields it is always 0.
 */
function fieldChanged(field, index) { }

function numeroSinistreChanged(field) {
  debug.print("numeroSinistreChanged DEBUT");
  document.setProperty(isIndexationValid, false);
  const champsIndexation = ["numeroDossier", "assigneGroup", "assigneRedacteur", "compteId"];
  champsIndexation.forEach(function (champ) {
    document.getField(champ).setValue("");
  });
  const numeroSinistre = field.value;
  debug.print("numeroSinistre: " + numeroSinistre);
  if (!/^\d+$/.test(numeroSinistre)) {
    debug.print("numeroSinistreChanged EXIT 1");
    return;
  }
  debug.print("numéro sinistre OK");
  const query = "SELECT TOP 1 [NO_DOSSIER], [Groupe], [numero_Adherent], [compte_nt] FROM [dbo].[V_AFFAIRE_PAPS] WHERE [Numero_Sinistre] = " + numeroSinistre;
  const queryResults = new DbServer('MAF BDD').query(query)
  if (queryResults.length !== 1) {
    debug.print("numeroSinistreChanged EXIT 2");
    return;
  }
  debug.print("V_AFFAIRE_PAPS from numéro sinistre OK");
  const queryResult = queryResults[0];
  const [numeroDossier, assigneGroup, compteId, redacteurCompteDomaineMaf] = queryResult;
  debug.print(JSON.stringify({
    numeroDossier: numeroDossier,
    assigneGroup: assigneGroup,
    compteId: compteId,
    redacteurCompteDomaineMaf: redacteurCompteDomaineMaf
  }));
  redacteurCompteDomaineMaf = redacteurCompteDomaineMaf.replace("\\+", "\\");
  const redacteurUserIdentifier = extract_user_identifier_from_MAF_NT_login(redacteurCompteDomaineMaf);
  debug.print(JSON.stringify({ redacteurUserIdentifier: redacteurUserIdentifier }));
  const redacteurUserCode =
    get_utilisateur_BUT_from_MAF_BUT_API({ mafDomainUserLogin: redacteurUserIdentifier })
      .codeUtilisateur;
  const indexationData = {
    numeroDossier: numeroDossier,
    assigneGroup: assigneGroup,
    assigneRedacteur: redacteurUserCode,
    compteId: compteId
  };
  debug.print("indexationData");
  debug.print(JSON.stringify(indexationData));
  for (var key in indexationData) {
    document.getField(key).setValue(indexationData[key]);
  }
  document.setProperty(
    "jsonDocumentMetadata",
    JSON.stringify(indexationData)
  );
  document.setProperty(isIndexationValid, true);
  debug.print("numeroSinistreChanged FIN");
}

/**
 * 
 * Called whenever the structure of a table is changed through the indexing UI (e.g. inserting/deleting rows).
 * 
 */
function tableChanged(table) { }

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
function fieldLoad(field) { }

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
function fieldSave(field) { }

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
function getFieldNamespace(step, node, field) { }

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
function fieldOcrCompleted(field, extractionData, maxConfidenceData) { }

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
function keyEvent(evt) { }

const isIndexationValid = "isIndexationValid";