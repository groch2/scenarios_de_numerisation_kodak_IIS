importClass(java.net.URL);
importClass(java.io.BufferedReader);
importClass(java.lang.StringBuffer);
importClass(java.util.UUID);

/**
 * Global script template.
 * 
 * This script is included in all other scripts (Job level and Index Class level scripts).
 * You can put here global functions that you want to be available in all other scripts.
 * 
 * This file also includes some functions that are global in nature.
 * You may either leave these methods in this template empty or remove them,
 * if you don't want to handle the specific events.
 */

/**
 * All scripts may run under different environments. A global variable 'runtime' 
 * exists in the context of all scripts and can take the following values:
 *   "client":     when running in the Java client either as an applet or as a
 *                 standalone Java application.
 *   "server":     when a script is executed by the server side as part of a server-side
 *                 workflow step.
 *   "importd":    when running in the importd service.
 *   "htmlclient": when running in the HTML client.
 * You may use function isJavaEngine() to distinguish the cases where a script is 
 * executed in a Java or a HTML environment.
 */
function isJavaEngine() {
  return (runtime != 'htmlclient');
}

if (isJavaEngine()) {
  importPackage(javax.swing);
  importPackage(java.lang);
  importPackage(java.awt);
  importPackage(java.awt.event);
  importPackage(Packages.scanclient.com4j.lib.wsh);
  importPackage(Packages.com.jacob.activeX);
  importPackage(Packages.com.jacob.com);
}

/**
 * *****************************************************
 * This function is available only with the JAVA client.
 * *****************************************************
 * This function is called to check whether the given password meets certain complexity 
 * standards. It is called when a new User is created or when a User's password 
 * is changed. Empty passwords are not allowed, but no restriction is applied to
 * the length or content of the password. If you wish to apply such restrictions, 
 * then you need to implement the following function.
 * 
 * Function isPasswordValid returns whether the given password is valid or not.
 * 
 * Note that this function is also responsible for displaying an appropriate error 
 * message to the User, in case the password is not valid.
 *
 * Example 1: Accept passwords that have at least one lowercase, one uppercase 
 *            letter and one number.
 * 
  if ( password.match(/[A-Z]/) && password.match(/[a-z]/) && password.match(/[0-9]/) ){
    return true;
  }
  else {
    alert('Valid passwords must have both lowercase and uppercase letters and at least one number');  
    return false;
  }
*
*/
function isPasswordValid(password) {
  return true;
}

/**
 * ******************************************************
 *  This function is available only with the JAVA client.
 * ******************************************************
 * 
 * This is called from within the 'New Batch' dialog every time the user
 * selects a new Job from the 'Job selection drop down list' (after the event).
 * It is called once with a 'null' jobName whenever the dialog is displayed (you can
 * use this 'check' to pre-select a job for example).
 * 
 * The 'dialog' is a reference to the actual JDialog java Object.
 *
 * The dialog object provides access to the components on the dialog, namely
 * fldJobs, fldName, fldDescr, fldPriority, fldTxtJobDescr, 
 * fldStatus, fldPrivate, fldCancel, fldCreate, fldCreateAndScan 
 * (also the associated labels: fldLblJob, fldLblName, fldLblDescr, fldLblPriority, 
 * fldLblStatus, fldLblJobDescr, fldLblPrivate).
 * Also to the fields associated with the scan profiles: scanProfilesPane,
 * fldScanProfiles, fldLblScanProfiles, fldMatchProfiles, fldImageMode,
 * fldLblImageMode, fldPageMode, fldLblPageMode, fldResolution, fldLblResolution.
 * Also to a custom panel (JPanel) that is below the job description: customPane:
 * this can be used to add custom components.
 * These are the actual Java Swing components whose properties and methods
 * are normally accessible.
 *
 * For example, to disable the 'Name' text box, one can write:
 *   dialog.fldName.enabled = jobName == 'My Job';
 *
 * To hide the 'Create' button:
 *   dialog.fldCreate.visible = false;
 *   
 * The dialog object provides methods scanProfilesPaneExpand() and 
 * scanProfilesPaneCollapse() that can be used to expand and collapse the 
 * Scan Profile pane. Normally, when the dialog is shown, the Scan Profile
 * pane is collapsed. To have the Scan Profiles pane expanded when the dialog 
 * is being displayed and the Job selection list default to 'no selection', 
 * you can use the following code:
 *  if (jobName == null){
 *    dialog.fldJobs.setSelectedIndex(-1);   
 *    dialog.scanProfilesPaneExpand();
 *    
 *    //create a unique batch name by evaluating a dynamic expression at runtime
 *    //using scripting (this could be done in the Job setup also)
 *    var batchName = ITUtils.eval("$CurrentDate $CurrentTime / ${Counter:HRRecs}");
 *    dialog.fldName.text = s;
 *  } 
 *
 * The dialog also provides method setScanProfileByName(String profileName)
 * that can be used to select a Scan Profile in the Scan Profiles combo box.
 * For example:
 *   if (jobName == 'Job1') {
 *     dialog.setScanProfileByName('myprofile');
 *   } 
 *
 * To retrieve the Jobs that are available on the Jobs combo box, you may use
 * method getJobs(), which returns a List of IJob objects.
 * For example:
 *   var jobs = dialog.getJobs();
 *   var job1 = jobs.get(0);
 *
 * Another example is to set a custom batch name. For example, one can make a
 * web-service call to get a batch name from another system and use it, e.g.:
 *   var newBatchName = .... //make a web service call to get a name
 *   dialog.fldName.text = newBatchName;
 *   dialog.fldPriority.value = 8;
 *   dialog.fldDescr.text = 'This is an important batch';
 *   
 */
function initNewBatchDialogForJob(dialog, jobName) {

}

/**
 * 
 * ******************************************************
 *  This function is available only with the HTML client.
 * ******************************************************
 * 
 * It allows the customization of the New Batch Dialog.
 * 
 * CUSTOMIZING EVENT LISTENERS
 * 
 * The 'listeners' object holds the following methods on specific events of the New Batch Dialog 
 * in order to be able to customize them and call them within custom code:
 * 
 * listeners.onCancel(defaultCancelFunction)
 * listeners.onCreate(defaultCreateFunction)
 * listeners.onCreateAndScan(defaultCreateAndScanFunction)
 * listeners.onJobSelected(defaultJobSelectedFunction)
 * 
 * These callbacks can be used to intercept the events and insert custom code.
 * 
 * - listeners.onCancel(defaultCancelFunction)
 * This is called when the user click on the 'Cancel' button or closes the dialog.
 * example:
 *  listeners.onCancel = function(defaultCancelFunction) {
 *    console.log("cancel button pressed");
 *
 *    if(confirm('are you sure you want to close?')) {
 *    defaultCancelFunction();
 *    };
 *  }
 * 
 * - listeners.onCreate(defaultCreateFunction)
 * This is called when the user has clicked the 'Create' button.
 * example:
 *  listeners.onCreate = function(defaultCreateFunction) {
 *    console.log("create button pressed");
 *    if(validateFields()) {
 *      defaultCreateFunction(); // 
 *    }
 *  }
 * 
 * - listeners.onCreateAndScan(defaultCreateAndScanFunction)
 * This is called when the user has clicked the 'Create And Scan' button.
 * example:
 *  listeners.onCreateAndScan = function(defaultCreateAndScanFunction) {
 *  console.log("createandscan button pressed");
 *  
 *    // do some custom checks or business logic here
 *    
 *    if(validateFields()) {
 *      defaultCreateAndScanFunction();
 *    }
 *  }
 * 
 * - listeners.onJobSelected(defaultJobSelectedFunction)
 * This is called everytime the user changes the selected job in the 'Job Selection' drop down box. It is useful
 * when it is necessary to run some custom code on job selection. For instance, to update other custom fields.
 * example:
 * listeners.onJobSelected = function(job) {
 *  console.log("job changed", job);
 *  myInput.value = 'job selected: ' + job.jobName; // fill in a custom field with the selected job name
 * }
 * 
 * CUSTOMIZING VISUALLY THE DEFAULT FIELDS OF THE DIALOG
 * 
 * All fields of the dialog are HTML inputs that can be changed by using javascript to change their contents 
 * and their the CSS styling as needed.
 * 
 * All the input fields of the dialog have distinct HTML IDs in order to be easily selecteable using javascript. Here 
 * is the list of the inputs and their corresponding IDs:
 * 
 * Dialog content div: 'NewBatchDialog'
 * 
 * Job Selection Dropdown: 'JobSelect'
 * Batch Status Dropdown: 'StatusSelect'
 * Job Name text input: 'JobName'
 * Job Priority numeric input: 'JobPriority'
 * Batch Private checkbox: 'CheckBoxPrivate'
 * Scan Profile Dropdown: 'ScanProfileSelect'
 * Scan Profile Match filter checkbox: 'CheckBoxMatchProfiles'
 * Scan Profile ImageMode Dropdown: 'ImageModeSelect'
 * Scan Profile Page Side Mode Dropdown: 'PageModeSelect'
 * Scan Profile Resolution editable Dropdown: 'ResolutionSelect'
 * 
 * Example:
 * 
 * // get a reference to the HTML dialog element
 * var dialogContent = document.getElementById('NewBatchDialog');
 * 
 * if(!dialogContent) {
 *  console.error("failed to find the NewBatchDialog element");
 *  return;
 * }
 * 
 * // get a reference to the Job Priority HTML input element
 *  var jobPriority = dialogContent.querySelector('#JobPriority');
 *  
 * // change the font and color of the Job Priority input
 *  jobPriority.style.fontSize = '20pt';
 * 
 * // add a custom field using DOM
 * var myDiv = document.createElement('div');
 * myDiv.id = 'myContent';
 *
 * // add the new content to the end of the new batch dialog content
 * dialogContent.appendChild(myDiv);
 *
 * // fill it with custom HTML by using innerHTML
 * myDiv.innerHTML += '<div id="myOneField"><label>My custom thing</label><input type="text" id="myInput"></div>'
 * + '<div id="myOtherField"><label>My other custom thing</label><input type="text" id="myOtherInput">*</div>' +
 *   '<small>* required field</small>';
 *
 *
 * // get their references
 * var myInput = document.getElementById('myInput');
 * var myOtherInput = document.getElementById('myOtherInput');
 *
 * // add custom css styles for them with a <style> tag
 * myDiv.innerHTML = '<style>' +
 *   '#myContent {' +
 *   '    background-color: bisque;\n' +
 *   '    padding: 0.5em;' +
 *   '}' +
 *   '#myContent > div {' +
 *   ' margin: 0.5em;' +
 *   '}' +
 *   '' +
 *   '#myContent label {' +
 *   '  margin: 0.5em; ' +
 *   '  display: inline-block;' +
 *   '  width: 25%;' +
 *   '}' +
 *   '#myInput {' +
 *   ' font-size: 15pt;' +
 *   ' width: 50%;' +
 *   '}' +
 *   '' +
 *   '#myOtherInput {' +
 *   ' font-weight: bold;' +
 *   ' color: green;' +
 *   ' width: 50%;' +
 *   '}' +
 *   '' +
 *   '</style>';
 *
 * CUSTOMIZING DEFAULT DROPDOWN ITEMS
 * 
 * The dropdown boxes are <input> elements that are associated with a <datalist> that contains <options> elements. 
 * These options can be changed directly by using simple javascript and HTML code and the drop down boxes will update
 * with the new options immediately.
 * Example:
 * 
 * // customize existing drop down items
 * console.log(jobSelection.list.options);
 * 
 * // get a reference to the first option of the dropdown
 * var jobName = jobSelection.list.options[0].innerText;
 * 
 * // change its content by updating it using innerHTML. The change will be immediately rendered
 * jobSelection.list.options[0].innerHTML = '<div style="font-weight: bold;">' 
 * + '<img src="/html-client/static/testimageballoon.jpg" style="width: 100px;">' 
 * + '<div style="font-weight: bold; margin: 1em 0; display: block;">(CUSTOMIZED) ' + jobName + '</div></div>';
 * 
 * @param {type} listeners 
 * @return {undefined}
 */
function customizeNewBatchDialog(listeners) {

}

/**
 * ******************************************************
 * This function is available only with the JAVA client.
 * ******************************************************
 * This function is called if a user drag-n-drops one or more files on the clients and there is
 * no current batch. You can use this function to create a new batch and return it and that
 * batch will be used to hold the new files that were dropped.
 * If you don't implement this function, then the standard 'New Batch' dialog will appear and
 * the user will be prompted to create a new batch.
 * 
 * The files argument is an array of standard java.io.File objects representing the files that
 * are being dropped. 
 * The jobs arguments is an array of IJob objects.
 * 
 * Example 1: the simplest possible implementation: creates blindly a batch from the first 
 *            available job:
             
  return jobs[0].createBatch();

 *  
 * Example 2: selects a job named 'eDocs' and creates a batch. It also assigns to the 
 *            batch the first status of this job:
             
  var JOB_NAME = 'eDocs';                    //this is the name of the job to select, 'eDocs'
  if (jobs.length > 0){                      //make sure there is at least one job       
    var job;
    for(i in jobs){                          //find the job with the name eDocs
      if (jobs[i].name == JOB_NAME){
        job = jobs[i];
        break;
      }
    }
    if (!isdefined(job))                     //if we don't find such a job, exit
      return;

    var batch = job.createBatch();           //create a new batch
    batch.name = "My Batch";                 //set batch name
    batch.description = "Some description";  //set description
    batch.priority = "3";                    //set priority
    err.println('assigning statuses...');
    if (job.statuses.length > 0){
      batch.status = job.statuses[0];        //set batch status to 1st available
    }
    return batch;                            //return the batch
  }  

 */
function filesDroppedWithNoBatch(files, jobs) {

}

/**
 * This function is called after the user has selected a Batch to open, either
 * for scanning or for indexing and before the Batch is locked at the server
 * and returned to client.
 * 
 * If this function returns false, then the Batch will not be opened. Note that 
 * in this case the application will not display any error message and the
 * dialog that displays the available Batches will remain open. This function
 * is executed on the EDT, so if you wish to block the opening of certain
 * Batches, then you can also display an appropriate message to the user.
 *
 * The argument 'parameters' has the following properties:
 *   offlineMode (boolean): true if the client operates in offline mode, false otherwise
 *   batchId (int): the id of the selected Batch 
 *   batchName (String): the name of the selected Batch
 *   batchDescription (String): the description of the selected Batch
 *   batchStatus (String): the status of the selected Batch
 *   batchCreationDate (java.util.Date): the creation date and time of the selected Batch
 *   batchPriority (int): the priority of the selected Batch
 *   batchPrivate (boolean): true if the selected Batch is private, false otherwise
 *   batchCreatorUsername (String): the username of the user that created the selected Batch
 *   stepActivity (String): the activity of the current step of the selected Batch
 *   stepName (String): the name of the current step of the selected Batch
 *   jobName (String): the name of the Job that the selected Batch belongs to 
 *   openedAsReadOnly (boolean): whether the batch has been requested to be opened as Read Only
 */
function batchWillOpen(parameters) {
  return true;
}

/**
 * This function is called when the new Batch action takes place. You should use this function,
 * to override the application's default new Batch dialog. This function must return information 
 * about how the new Batch should be created.
 * 
 * The jobs argument is an array of IJob objects.
 * The scanProfiles argument is an array of IScanProfile objects and it holds all local 
 * Scan Profiles.
 * 
 * The returned object is a Map that has the following attributes:
 *   actionName :      This is a mandatory String attribute and it can be "CREATE", "SCAN", or "CANCEL".
 *                     If CREATE is returned, then the new Batch will be created and displayed.
 *                     If SCAN is returned, then the new Batch will be created and scanning will begin.
 *                     If CANCEL is returned, then no Batch is created and the application remains empty.
 *   jobName:          If the actionName is "CREATE" or "SCAN", then this is a mandatory attribute 
 *                     and it is a String. It must specify the name of the Job that should be used
 *                     for creating the new Batch.
 *   batchName:        A String to use as the name of the new Batch. It may contain variables.
 *   batchDescription: A String to use as the description of the new Batch. It may contain variables.
 *   batchStatus:      A String to use as the status of the new Batch. If it does not match one of 
 *                     statuses assigned on the selected Job, it will be ignored.
 *   batchPrivate:     A boolean that specifies whether the new Batch will be private.
 *   batchPriority:    An integer that specifies the priority of the new Batch.
 *   batchProperties:  A map that contains properties to be assigned on the new Batch. The map
 *                     should contain String keys and String values.
 *   scanProfileName:  A String that specifies the name of the Scan Profile to use for scanning.
 *   scanImageMode:    A String that specifies the Image mode to use for scanning.
 *                     It must be "COLOR", "GRAY" or "BINARY".
 *   scanPageMode:     A String that specifies the Page mode to use for scanning.
 *                     It must be "SINGLE_SIDE" or "DUPLEX".
 *   scanResolution:   An integer that specifies the resolution (in dpi) to use for scanning.
 *                
 * If this function is not implemented, or it returns null, or an error happens, then the 
 * application will display the default new Batch dialog in order to allow the user to select
 * how to create the new Batch.
 * 
 * Example implementation:
   return {
     actionName : 'CREATE',
     jobName : 'Job1',
     batchPrivate : false,
     batchPriority : 10,
     batchProperties : { 
       prop1 : 'value1',
       prop2 : 'value2'
     }
   }
 */
function newBatch(jobs, scanProfiles) {
  return null;
}

/**
 * This function is called after the application has successfully been initialized
 * and all required data has been loaded. 
 * In Transactional mode this function is not called.
 * Arguments:
 *   currentBatch: Type is com.imagetrust.tc.model.object.IBatch. If during startup, 
 *      a Batch was found and loaded from the local disk, this will be a reference 
 *      to the current Batch, otherwise it will be null.
 */
function ready(currentBatch) {
}

/**
 * Some general utility functions
 */
function alert(s) {
  if (isJavaEngine())
    JOptionPane.showMessageDialog(MainWindow, s);
  else
    window.alert(s);
}

function isdefined(v) {
  return (typeof (v) == "undefined") ? false : true;
}

String.prototype.strip = function () {
  return this.replace(/[\\\/]/g, '_');
}

if (isJavaEngine()) {
  String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, "");
  }
}

String.prototype.left = function (idx) {
  return this.substring(0, idx);
}

function compareStringsCaseInsensitive(a, b) {
  return a.localeCompare(b, undefined, { sensitivity: 'accent' });
}

function areStringsEqualsCaseInsensitive(a, b) {
  return compareStringsCaseInsensitive(a, b) === 0;
}

// https://github.com/jsPolyfill/Array.prototype.find/blob/master/find.js
Array.prototype.find = Array.prototype.find || function (callback) {
  if (this === null) {
    throw new TypeError('Array.prototype.find called on null or undefined');
  } else if (typeof callback !== 'function') {
    throw new TypeError('callback must be a function');
  }
  var list = Object(this);
  // Makes sures is always has an positive integer as length.
  var length = list.length >>> 0;
  var thisArg = arguments[1];
  for (var i = 0; i < length; i++) {
    var element = list[i];
    if (callback.call(thisArg, element, i, list)) {
      return element;
    }
  }
};

/**
 * String.prototype.replaceAll() polyfill
 * https://gomakethings.com/how-to-replace-a-section-of-a-string-with-another-one-with-vanilla-js/
 * @author Chris Ferdinandi
 * @license MIT
 */
if (!String.prototype.replaceAll) {
  String.prototype.replaceAll = function (str, newStr) {
    return (
      Object.prototype.toString.call(str).toLowerCase() === '[object regexp]' ?
        this.replace(str, newStr) :
        this.replace(new RegExp(str, 'g'), newStr));
  };
}

/**
 * String.prototype.padStart() polyfill
 * https://github.com/uxitten/polyfill/blob/master/string.polyfill.js
 * https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/padStart
 */
if (!String.prototype.padStart) {
  String.prototype.padStart = function padStart(targetLength, padString) {
    targetLength = targetLength >> 0; //truncate if number or convert non-number to 0;
    padString = String((typeof padString !== 'undefined' ? padString : ' '));
    if (this.length > targetLength) {
      return String(this);
    }
    else {
      targetLength = targetLength - this.length;
      if (targetLength > padString.length) {
        padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
      }
      return padString.slice(0, targetLength) + String(this);
    }
  };
}

function httpGetString(url) {
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
