/**
 * Job script template
 * 
 * You may either leave the functions in this template empty or remove them,
 * if you don't want to handle the specific events.
 */

/** 
 * NOTICE: the initNewBatchDialogForJob() function was moved to the Global script 
 * from version 2.6
 */

debug.print("task - global - test log");

/**
 * Called after a new Batch is created. The job level scripting is evaluated
 * once for each new batch, so this is the first function called after 
 * script evaluation.
 * This is the place to initialize any batch level index fields.
 * Notice that when a batch is created, an empty folder and document are
 * automatically added.
 * 
 * You can set the job level properties here:
 * - inPlaceFiletypes: array of strings: 
 *   file extensions for types of eDocs that you want to be displayed as 
 *   text files in-line, e.g.
 *     if (isJavaEngine()) batch.job.setInPlaceFileTypes(["ini", "txt", "xml", "html"]);
 *   
 *   Default value is: ["txt", "xml", "reg", "bat", "ini"]
 *   NOTICE: This method (setInPlaceFileTypes) is not supported in HTML-Client
 * 
 */
function batchCreated(batch) {
  debug.print("task - batchCreated - test log");
}

/**
 * This function is called when a Batch is loaded on the client.
 */
function batchLoaded(batch) {
  debug.print("task - batchLoaded - test log");
}

/**
 * This function is called when a Batch is unloaded on the client.
 * You may use this function to invoke another action (for example NewBatch),
 * but you should pay attention to the value of actionFollows.
 * Arguments:
 *   batch: Type is com.imagetrust.tc.model.object.IBatch. 
 *     The Batch that was unloaded.
 *   reason: Type is com.imagetrust.tc.model.object.UnloadReason. 
 *     It indicates the reason that caused the unloading of the Batch.
 *   actionFollows: Type is com.imagetrust.tc.model.object.ActionType.
 *     It indicates whether the unloading of the Batch is followed by
 *     another action. For example, if while a Batch is loaded on the
 *     application, the user decides to create a new Batch, he/she will
 *     be asked to decide whether to close or suspend the current Batch. 
 *     If the current Batch is closed, function batchUnloaded will be called
 *     with reason UnloadReason.Close and actionFollows ActionType.NewBatch.
 */
function batchUnloaded(batch, reason, actionFollows) { }

/**
 * Called every time the structure of the Batch changes.
 * Notice that this event may be called more than once for a 'single' user
 * operation (for example, when a page is moved from one document to another, 
 * it is called more than once, since the page is first removed from the 
 * first document and then inserted to the other).
 */
function batchStructureChanged(batch) { }

/**
 * Called before starting a scan operation
 * Return 'Const.AbortOp' or 'false' if you want to abort scanning.
 *   
 * Notice that you can use the 'ReturnValue' property to set the return value
 * of the function, e.g.
 *   return {
 *     ReturnValue: true
 *   };
 *
 * This callback also allows creating custom file extension filters
 * in the native "Browse Files" dialog that opens when importing from
 * the file system.
 *
 * return {
 *  FileBrowserVisibleTypes: ["pdf", "tif", "tiff", "jpeg", "jpg"]
 * };
 *
 */
function preScan(batch) {
  debug.print("task - preScan - test log");
}

/**
 * Called after a scan operation finishes
 */
function postScan(batch) {
  debug.print("task - postScan - test log");
}

/**
 * This function is called before the pageScanned() function and before the
 * page is added to the batch. It should be used to provide custom separation
 * logic as it is called before the separation rules are executed.
 * 
 * IMPLEMENTING CUSTOM SEPARATION LOGIC:
 * By default, the separation rules will be executed after this function returns.
 * To provide custom handling you should return an object that contains a custom
 * separation result or that suppresses the execution of the separation rules.
 * For example:
 *   ...
 *   return { 
 *     Separation: Const.SepNone, 
 *     Retention: Const.KeepPage
 *   };
 * 
 * The available values for the Separation property are:
 *  - Const.SepDefault: execute separation rules (default value)
 *  - Const.SepNone: suppress execution of separation rules
 *  - Const.SepNewDoc: create a new document
 *  - Const.SepNewFolder: create a new folder
 *  
 * The available values for the Retention property are:
 *  - Const.KeepImage:  keep the scanned image (default value)
 *  - Const.DeleteImage: delete the scanned image
 *  - Const.DeletePage: delete the scanned image and the back side (if duplex scanning)
 * 
 * EXAMPLE 1: Delete all blank  (lesss than 3% black pixels) pages as they arrive:
 *   function pageArrived(batch, page){
 *     var blank = page.isBlank();
 *     if (blank == null){
 *       blank = page.detectIsBlank(0.03);
 *       if (blank)
 *         return { Separation: Const.SepNone, Retention: Const.DeleteImage }
 *     }
 *   }
 *   
 *   
 * EXAMPLE 2: Print the values of all Code39 barcodes detected and create
 *            a new document if the second barcode has the value 'BREAKHERE'.
 *            Delete the image with the barcode.
 *   ...
 *   //outside the function
 *   if (isJavaEngine()) importPackage(com.imagetrust.tc.model.object);
 *   
 *   ...
 *   
 *   //declare the Code_39 barcode to be detected during scanning
 *   function preScan(batch){
 *     return { DetectBarcodeTypesWhileScanning: [BarcodeType.Code_39] };
 *   }
 *   
 *   ...
 *   
 *   function pageArrived(batch, page){
 *     for(var i in page.barcodeData){
 *       var bcd = page.barcodeData[i];
 *       if (bcd.value != null){
 *         if((i==1) && bcd.value == 'BREAKHERE') {
 *          return { Separation: Const.SepNewDoc, Retention: Const.DeleteImage }
 *         }
 *       }
 *     } 
 *   }
 *
 */
function pageArrived(batch, page) { }

/**
 * Called after a page is scanned. This event is called AFTER the page
 * is added to the document.
 * If the page is deleted (e.g. blank page or for another reason), this function
 * is never called for this page.
 * 
 * Return Const.AbortOp to abort scanning (aborting the scanning operation
 * may cause a few more pages that are buffered to come through).
 * NOTICE: do NOT display any dialog from this function because it is 
 * run in a different thread while scanning and it will NOT block the GUI.
 * Example code:
 *   if (batch.pageCount>=2){
 *     return Const.AbortOp;
 *   }
 *
 * Parameters:
 * batch: the current batch
 * document: the current document that the page is added to
 * page: the current page that was just scanned
 */
function pageScanned(batch, document, page) { }

/**
 * This function is called when the user tries to close a batch. It can be used in
 * order to override the default checks that prevent a batch from closing. 
 * This function is not called when the user tries to suspend a batch.
 * 
 * By default, if a batch contains rejected nodes, it cannot be closed. If you
 * want to suppress the check for rejected nodes, then you can return the
 * following:
 *   return {
 *     AllowRejectedNodes: true
 *   }
 * 
 * By default, if a batch is in an indexing step and all required index fields 
 * are not valid, then it cannot be closed. If you want to suppress the check
 * for invalid required fields, then you can return the following:
 *   return {
 *     AllowIncompleteIndexing: true
 *   }
 */
function batchCanBeClosed(batch) { }

/**
 * This function is called just before the batch is about to be closed or suspended 
 * and before the standard warning dialog is displayed to the user 
 * (that asks him if he really wants to close/suspen the batch).
 * Parameters:
 *  batch: the batch that is about to be closed (type is 
 *    com.imagetrust.tc.model.object.IBatch)
 *  suspend: true if the batch will be suspended and false if it will be closed
 *  closeContext: provides information on the context that prompted the invocation
 *    of this function (type is com.imagetrust.tc.model.object.CallContext)
 *  
 * Return 'Const.AbortOp' or 'false' if you don't want the batch to close.
 * Set the property 'SuppressWarnDlg' to true to suppress the standard warning dialog.
 * For example: 
 *   return {
 *     ReturnValue: Const.AbortOp,
 *     SuppressWarnDlg: true
 *   }
 *   
 * By default the batch is uploaded asynchronously to the server. If you want to 
 * upload the batch synchronously, then you can set the property 'UploadMode' to
 * 'Const.Sync'. For example:
 *   return { UploadMode: Const.Sync }
 * Note that synchronous upload is not allowed when the batch is closed/suspended
 * following the detection of a batch separation sheet.
 * 
 * When a split batch action takes place, the batch is first saved to the server.
 * Before this happens, function batchWillClose is called with callContext BeforeSplit.
 * After the batch is successfully saved to the server, it is split based on
 * the user's selections and function batchWillClose is called for both the original 
 * and the new batch. At that point the callContext is AfterSplit and
 * no modifications can be made to the original or the new batch (node properties,
 * index field data, etc). The sole purpose of calling function batchWillClose on 
 * the original and the new batch, is in order to enforce custom rules and to abort 
 * the split action if these rules are not met.
 * 
 * EXAMPLE 1: Ask for user confirmation if Batch has too many pages.
 *  function batchWillClose(batch, suspend){
 *    if (isJavaEngine()) {
 *      var MAXPAGES = 2;
 *      var rv;
 *      if (batch.pageCount >= MAXPAGES){
 *        rv = JOptionPane.showConfirmDialog(MainWindow,
 *          "This batch has more than " + MAXPAGES + " pages. Are you sure you want to close it?",
 *          "WARNING: Batch is too big, better split it up!",
 *          JOptionPane.YES_NO_OPTION,
 *          JOptionPane.QUESTION_MESSAGE);
 *        if ( rv == JOptionPane.NO_OPTION )
 *          return { ReturnValue: Const.AbortOp, SuppressWarnDlg: true }
 *      }
 *    }
 *  }
 */
function batchWillClose(batch, suspend, closeContext) { }

/**
 * This function is called each time the local (client-embedded) OCR engine completes
 * the OCR operation for an index field asynchronously.
 * Since the OCR is performed asynchronously for all documents when the user 
 * starts Indexing, this function will be called for every document in the 
 * current batch for which the Document class is set (or for all documents
 * if the current Job only contains a single Document class).
 * 
 * Note that it is possible to define more than one index zones for the same
 * index field. This function is called once, when the ocr for all related index
 * zones is complete. The function provides a list that contains extracted data
 * from all index zones. As a convenience it also provides the extracted
 * data with the highest confidence value.
 * 
 * You could write code here to pre-fill the field values of documents that are
 * not currently being indexed as the OCR is performed for each field,
 * using code like this:
 * 
 *   if (!isCurrentIndexingNode && maxConfidenceData != null && field.value == ''){
 *     field.parsedValue = maxConfidenceData.value;
 *     field.review = maxConfidenceData.confidence < maxConfidenceData.indexZone.minConfidence ? true : false;
 *   }
 *   
 * Since this code is running outside the context of an Index step configuration,
 * the "field.value" maps to the default namespace which is used to save field values, 
 * which is the "idx.<field_name>.value". If you want to store the extracted
 * value in another namespace, you should define it accordingly. An exception to this
 * is when the isCurrentIndexingNode parameter is true: this is the case that the
 * function it called for the current index node, in which case the "field.value"
 * maps to the transient value of the field that it is used during indexing. 
 * 
 * WARNING:
 * It it suggested that you do NOT write any field values for the current indexing node,
 * e.g. always check the isCurrentIndexingNode==false before writing properties to the field.
 * There is absolutely no guarantee on the order in which this function will be called
 * in relation to the fieldOcrCompleted() function the Index script. To alter the
 * behavior for the ocr of the current index node, you should use the fieldOcrCompleted() function
 * in the index script of the appropriate index class.
 *   
 * Notice that the above code snippet also sets the 'review' attibute to 'true'
 * if the ocr confidence is lower than the one defined for the zone.
 * 
 * Parameters:
 *   document: the document that the ocr was performed on.
 *   field : the index field for which the ocr was performed.
 *   extractionData : a list of all the extracted data, from all index zones 
 *     that refer to the given index field.
 *   maxConfidenceData: the extracted data with the highest confidence.
 *     If the index field uses only one index zone, then the maxConfidenceData
 *     will be equal to the only item in the extractionData list.
 *   isCurrentIndexingNode: a boolean value that is true when the given document
 *     is the node that is currently being indexed, false otherwise.
 */
function fieldOcrCompleted(document, field, extractionData, maxConfidenceData, isCurrentIndexingNode) {
  debug.print("task - fieldOcrCompleted - test log");
}

/**
 * This function is called whenever the OCR engine start or stops background OCR work.
 */
function zoneOcrInProgress(working) {
  debug.print("task - zoneOcrInProgress - test log");
}

/**
 * Called whenever index mode start/finished on the client. 
 * This function corresponds mostly to the GUI state
 * 
 * When index-mode finishes (e.g. start == false), then you may set the 
 * SwithToScanActivity property to 'true' to force the client to revert
 * back to the SCAN activity, if initially the batch was in that activity.
 * For example:
 *   return { SwitchToScanActivity: true }
 * 
 * Parameters:
 *   start: true if index mode starts, false if it ends
 *   batch: the current batch
 */
function setIndexMode(start, batch) { }

/**
 * Called whenever indexing starts for a node. This function is called before
 * any of the indexing-level scripts have a chance to execute.
 * The node will either be a batch, folder or document. As the user moves through
 * a batch, this function is called before each node becomes the active indexing node.
 * 
 * This function may be used to set the index-class or form-type for a node automatically.
 * 
 * Parameters:
 *   node: the node that is about to be indexed
 */
function startNodeIndexing(node) {
  debug.print("task - startNodeIndexing - test log");
}

/**
 * Called whenever the indexing ends for a node. This function is called after
 * all indexing-level scripts are executed.
 * 
 * You may choose to disallow indexing to end by returning 'Const.Abort' or 'false',
 * for example:
 *  if (!forceEnd && node.isAtLeastOneFieldEmptyOrInvalid(false)){
 *    alert('Complete indexing before moving on');
 *    return false;
 *  }
 *   
 * Make sure that you check that forceEnd==false if you want to dis-allow indexing
 * to stop.
 * 
 * Parameters:
 *   node: the node being indexed
 *   forceEnd: this is normally false, unless indexing is required to stop (this
 *     currently happens if the user chooses to 'Discard' the current batch, in 
 *     which case all changes are reverted, so there is no reason to not stop
 *     indexing). 
 */
function endNodeIndexing(node, forceEnd) { }

/**
 * Called each time a context-menu, or another action that may modify the batch (e.g
 * when mouse moves between two pages in the multi-document viewer and the split-document
 * indicator appears) in any way is about to occur. 
 * This function allows to veto (e.g. disallow) one or more actions of a batch.
 * This function should return 'false' if you want an action to be disabled.
 * 
 * Parameters:
 *  actionType: the action that it is queried (type is com.imagetrust.tc.model.object.ActionType).
 *  actionProperties: this is a map of extra properties that carry extra context information
 *    for the action. They depend on the actionType. For example, if actionType is 'RotatePage',
 *    then actionProperties contain the 'rotation_angle' property that may take the value [90,180,270].
 *  actionNode: this is the node on which the action is being performed on
 *  selectedNodes: this is a list of all currently selected nodes  
 * 
 * For example, if you want to disallow the duplication of pages:
 *   if (actionType == 'DuplicatePages')
 *     return false;
 */
function isContextActionEnabled(actionType, actionProperties, actionNode, selectedNodes) { }

/**
 * This function allows the apply of custom logic before a file is imported from the file system to the application.
 *
 * The file's mime type, extension and size (in bytes) will be provided as arguments to this function and
 * a boolean value is expected as a response in order to continue with the importing process
 * or notify the user that the file was blocked. If a string value is returned, the import will be <b>blocked</b>
 * and the value will be in the information dialog that will be shown to the user.
 * 
 * The mime type checking happens by inspecting the file itself and does not rely in its extension
 * in the file name. If the file inspection returns an unknown file, then the mimeType argument
 * will have a value of 'undefined'.
 *
 * The extension is extracted from the file name itself. This allows for checking for
 * extension-mime discrepancies and possibly tweaked files.
 *
 * Note that you are responsible for handling upper and lowercase file extensions.
 *
 * If the file inspection and/or the extension extraction from the file name fail then the file
 * will not be imported.
 */
function checkFileAllowed(mimeType, extension, fileSize) { }
