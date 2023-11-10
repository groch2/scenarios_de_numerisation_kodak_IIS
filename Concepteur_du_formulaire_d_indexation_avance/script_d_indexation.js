importPackage(Packages.com.imagetrust.tc.model.object);
importPackage(Packages.scanserver.released.rdpi);
importPackage(Packages.scancommon.model.release);
importClass(java.io.OutputStream);
importClass(java.io.OutputStreamWriter);
importClass(javax.net.ssl.HttpsURLConnection);
importClass(java.io.IOException);
importClass(java.io.BufferedReader);
importClass(java.lang.StringBuilder);
importClass(java.io.InputStreamReader);
importClass(java.security.MessageDigest);
importClass(javax.xml.bind.DatatypeConverter);
importClass(java.io.File);
importClass(java.util.concurrent.TimeUnit);
importClass(java.io.PrintWriter);
importClass(java.io.FileInputStream);
importPackage(java.io);
importPackage(java.net);
importPackage(javax.net.ssl);
importPackage(java.lang);
importPackage(java.security);
importPackage(javax.xml.bind);
importPackage(java.util);

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
function load(batch) {
  debug.print("début de la fonction 'load' du script d'indexation");

  debug.print("fonctions array disponibles");
  debug.print("at : " + Array.prototype.at);
  debug.print("concat : " + Array.prototype.concat);
  debug.print("copyWithin : " + Array.prototype.copyWithin);
  debug.print("entries : " + Array.prototype.entries);
  debug.print("every : " + Array.prototype.every);
  debug.print("fill : " + Array.prototype.fill);
  debug.print("filter : " + Array.prototype.filter);
  debug.print("find : " + Array.prototype.find);
  debug.print("findIndex : " + Array.prototype.findIndex);
  debug.print("findLast : " + Array.prototype.findLast);
  debug.print("findLastIndex : " + Array.prototype.findLastIndex);
  debug.print("flat : " + Array.prototype.flat);
  debug.print("flatMap : " + Array.prototype.flatMap);
  debug.print("forEach : " + Array.prototype.forEach);
  debug.print("includes : " + Array.prototype.includes);
  debug.print("indexOf : " + Array.prototype.indexOf);
  debug.print("join : " + Array.prototype.join);
  debug.print("keys : " + Array.prototype.keys);
  debug.print("lastIndexOf : " + Array.prototype.lastIndexOf);
  debug.print("map : " + Array.prototype.map);
  debug.print("pop : " + Array.prototype.pop);
  debug.print("push : " + Array.prototype.push);
  debug.print("reduce : " + Array.prototype.reduce);
  debug.print("reduceRight : " + Array.prototype.reduceRight);
  debug.print("reverse : " + Array.prototype.reverse);
  debug.print("shift : " + Array.prototype.shift);
  debug.print("slice : " + Array.prototype.slice);
  debug.print("some : " + Array.prototype.some);
  debug.print("sort : " + Array.prototype.sort);
  debug.print("splice : " + Array.prototype.splice);
  debug.print("toLocaleString : " + Array.prototype.toLocaleString);
  debug.print("toReversed : " + Array.prototype.toReversed);
  debug.print("toSorted : " + Array.prototype.toSorted);
  debug.print("toSpliced : " + Array.prototype.toSpliced);
  debug.print("toString : " + Array.prototype.toString);
  debug.print("unshift : " + Array.prototype.unshift);
  debug.print("values : " + Array.prototype.values);
  debug.print("with : " + Array.prototype.with);

  this.familles = JSON.parse(httpGetRequest("https://api-ged-intra.int.maf.local/v2/Familles?%24select=familleDocumentId%2Ccode%2Clibelle&%24filter=isActif%20eq%20true")).value;
  this.cotes = JSON.parse(httpGetRequest("https://api-ged-intra.int.maf.local/v2/Cotes?%24select=coteDocumentId%2Ccode%2Clibelle%2CfamilleDocumentId&%24filter=isActif%20eq%20true")).value;
  this.typesDocument = JSON.parse(httpGetRequest("https://api-ged-intra.int.maf.local/v2/TypesDocuments?%24select=typeDocumentId%2Ccode%2Clibelle%2CcoteDocumentId&%24filter=isActif%20eq%20true")).value;
  debug.print("récupération des tryptiques terminée");

  debug.print("nombre de familles chargées dans la fonction 'load'");
  debug.print(this.familles.length);
  printOutputSeparator();

  debug.print("données des familles de document depuis la fonction 'load'");
  printArrayOfObjects(this.familles);
  printOutputSeparator();

  /*
  printArrayOfObjects(cotes);
  debug.print("fin de l'affichage des cotes");
  debug.print("");

  printArrayOfObjects(typesDocument);
  debug.print("fin de l'affichage des types de document");
  debug.print("");
  */

  debug.print("fin de la fonction 'load' du script d'indexation");
  printOutputSeparator();
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
  // debug.print("argument 'node' de la fonction 'preProcess'")
  // printPropertiesValuesOfObject(node);
  // printOutputSeparator();

  debug.print("données des familles de document depuis la fonction 'preProcess'");
  printArrayOfObjects(this.familles);
  printOutputSeparator();

  const familles = [];
  for (var index = 0; index < this.familles.length; index++) {
    var famille = this.familles[index];
    familles.push(["" + famille.familleDocumentId, famille.libelle]);
  }
  debug.print("nombre de familles pour alimenter la liste déroulante :");
  debug.print(familles.length);
  printOutputSeparator();

  debug.print("source de données pour la liste déroulante des familles")
  printArrayOfObjects(familles);
  printOutputSeparator();

  debug.print("champ 'familles'");
  printPropertiesOfObject(node.fields['famille']);
  printOutputSeparator();

  fillDropDownField(node.fields['famille'], familles);
}

/**
 * Called after finishing indexing on a node of this class
 */
function postProcess(node) {
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
  return true;
}

/**
 * Called every time the value of a field is changed.
 * You may use the markValid() and markInvalid() methods to mark whether the
 * value of the field is valid or not.
 * 
 * For multivalued fields <code>index</code> is the index of the value loosing the focus, for normal fields it is always 0.
 */
function fieldChanged(field) {
  if (
    findItemInArrayByPredicate(
      ["famille", "cote", "type_document", "date_document"],
      function (item) { return areStringsEqualsCaseInsensitive(field.name, item) }) === null) {
    return;
  }
}

function familleChanged(field, index) {
  debug.print("famille sélectionnée: " + field.value);
  debug.print("élément sélectionné :");
  debug.print(field.valueAsListItem || "");
  const familleDocumentId =
    findItemInArrayByPredicate(
      this.familles,
      function (famille) {
        return areStringsEqualsCaseInsensitive(famille.libelle, field.value);
      })
      .familleDocumentId;
  debug.print("selected familleDocumentId: " + familleDocumentId);
  const cotesOfSelectedFamille = this.cotes.filter(function (cote) { return cote.familleDocumentId === familleDocumentId; });
  debug.print("cotes sélectionnées d'après la famille OK");
  debug.print(JSON.stringify(cotesOfSelectedFamille));

  const cotes = [];
  for (var index = 0; index < cotesOfSelectedFamille.length; index++) {
    var cote = cotesOfSelectedFamille[index];
    cotes.push(["" + cote.coteDocumentId, cote.libelle]);
  }
  debug.print("cotes pour la liste déroulante :");
  debug.print(JSON.stringify(cotes));

  fillDropDownField(node.fields['cote'], cotes);
}

function coteChanged(field, index) {
  const coteDocumentId =
    findItemInArrayByPredicate(
      this.cotes,
      function (cote) {
        return areStringsEqualsCaseInsensitive(cote.libelle, field.value);
      })
      .coteDocumentId;
  debug.print("selected coteDocumentId: " + coteDocumentId);
  const typesOfSelectedCote = this.typesDocument.filter(function (type) { return type.coteDocumentId === coteDocumentId; });
  debug.print("types sélectionnées d'après la cote OK");
  debug.print(JSON.stringify(typesOfSelectedCote));

  const types = [];
  for (var index = 0; index < typesOfSelectedCote.length; index++) {
    var type = typesOfSelectedCote[index];
    types.push(["" + type.coteDocumentId, type.libelle]);
  }
  debug.print("types pour la liste déroulante :");
  debug.print(JSON.stringify(types));

  fillDropDownField(node.fields['type_document'], types);
}

function date_documentChanged(field) {
  debug.print("date document: " + field.value);
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

function httpGetRequest(url) {
  const urlConnection = new URL(url).openConnection();
  urlConnection.setUseCaches(true);
  urlConnection.setRequestMethod("GET");
  urlConnection.setConnectTimeout(1000);
  // const responseCode = urlConnection.getResponseCode();
  const bufferedReader = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
  var inputLine;
  const stringBuffer = new StringBuffer();
  while ((inputLine = bufferedReader.readLine()) !== null) {
    stringBuffer.append(inputLine);
  }
  bufferedReader.close();
  urlConnection.disconnect();
  return stringBuffer.toString();
}

function fillDropDownField(dropDownField, tuplesList) {
  debug.print("nombre de données à charger dans la liste déroulante");
  debug.print(tuplesList.length);
  printOutputSeparator()

  tuplesList.sort(function (a, b) { return compareStringsCaseInsensitive(a[1], b[1]); });

  debug.print("test des données à charger dans la liste déroutante");
  debug.print(JSON.stringify(tuplesList));
  printOutputSeparator();

  dropDownField.clearOptions();
  for (var index = 0; index < tuplesList.length; index++) {
    var tuple = tuplesList[index];
    dropDownField.addOption(tuple[0], tuple[1]);
  }
}

function printArrayOfObjects(arrayOfObjects) {
  for (var index in arrayOfObjects) {
    for (var property in arrayOfObjects[index]) {
      debug.print(property + ": " + arrayOfObjects[index][property]);
    }
  }
}

function printPropertiesOfObject(object) {
  for (var property in object) {
    debug.print(property + ": " + object[property]);
  }
}

function printOutputSeparator() {
  debug.print("_____________________________________________________________________________________________");
}

function compareStringsCaseInsensitive(a, b) {
  return a.localeCompare(b, undefined, { sensitivity: 'accent' });
}

function areStringsEqualsCaseInsensitive(a, b) {
  return compareStringsCaseInsensitive(a, b) === 0;
}

function findItemInArrayByPredicate(array, predicate) {
  for (var i = 0; i < array.length; i++) {
    if (predicate(array[i])) {
      return array[i];
    }
  }
  return null;
}