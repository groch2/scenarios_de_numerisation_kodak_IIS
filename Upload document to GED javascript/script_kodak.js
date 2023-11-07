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

function release(context) {
  var document = context.getReleaseItem();

  //get docID
  var documentId = document.getId();
  log.info("documentId : " + documentId);
  var boundary = "" + System.currentTimeMillis();
  log.error("Boundary: " + boundary);

  // do web service post
  var requestURL = "https://api-ged-intra.int.maf.local/v2/upload";

  var url = new URL(requestURL);
  var httpConn = url.openConnection();
  var fieldName = "";

  httpConn.setUseCaches(false);
  httpConn.setDoOutput(true); // indicates POST method
  httpConn.setDoInput(true);
  httpConn.setRequestProperty("Content-Type", "multipart/form-data; boundary=" + boundary);
  httpConn.setRequestProperty("Accept", "application/json; utf-8");
  httpConn.setRequestProperty("Accept", "*/*");
  httpConn.setRequestMethod("POST");
  httpConn.setConnectTimeout(1000);

  /*  // write the json and get response code
  var os = new OutputStreamWriter(httpConn.getOutputStream());
     
  os.write(json1, 0, json1.length);
  os.flush();
  var msg = httpConn.getResponseMessage();
  var status =httpConn.getResponseCode(); */

  var docFiles = context.getSharedObject(ImagesReleaseCommon.OUTPARAM_FILES);

  log.error("docFiles: " + docFiles);
  var uploadFile = new File(docFiles[documentId][0]);
  // get file name
  var fileName = uploadFile.getName();

  // create output stream
  var outputStream = httpConn.getOutputStream();

  // create writer
  var writer = new PrintWriter(new OutputStreamWriter(outputStream, "utf-8"), true);

  // write boundary
  writer.append("--" + boundary).append("\r\n");

  // Content-Disposition passing name (ScannedFile) and filename
  writer.append(
    "Content-Disposition: form-data; name=\"" + fieldName
    + "\"; filename=\"" + fileName + "\"")
    .append("\r\n");

  // write Content-Type as PDF    
  writer.append(
    "Content-Type: application/tif")
    //+ URLConnection.guessContentTypeFromName(fileName))
    .append("\r\n");

  // get encoding to binary
  writer.append("Content-Transfer-Encoding: binary").append("\r\n");
  writer.append("\r\n");
  writer.flush();

  // create file input stream and write to output stream
  var inputStream = new FileInputStream(uploadFile);

  var buffer = java.lang.reflect.Array.newInstance(java.lang.Byte.TYPE, 4096)
  var bytesRead = -1;
  while ((bytesRead = inputStream.read(buffer)) != -1) {
    outputStream.write(buffer, 0, bytesRead);
  }
  outputStream.flush();
  inputStream.close();

  writer.append("\r\n");
  writer.flush();

  writer.append("\r\n").flush();
  writer.append("--" + boundary + "--").append("\r\n");
  writer.close();

  var msg = httpConn.getResponseMessage();

  var status = httpConn.getResponseCode();
  if (status != 200) {
    log.error("Error in POST Upload API: " + status + " " + httpConn.getResponseMessage());
    throw new Exception();
  }

  log.error("POST Upload response OK");
  log.error("Mess " + msg);

  var br = new BufferedReader(new InputStreamReader(httpConn.getInputStream()));
  var sb = new StringBuilder();
  var line = "";
  while ((line = br.readLine()) != null) {
    sb.append(line + "\n");
  }
  br.close();
  log.error("GuiID Normalement " + sb.toString());
  var json = JSON.parse(sb.toString())
  log.error("guidFile : " + json.guidFile);

  // create json
  const strJson = JSON.stringify({
    "fileId": json.guidFile,
    "libelle": "test.pdf",
    "deposePar": "ROD",
    "dateDocument": "2023-11-06T16:37:36.4772705Z",
    "fichierNom": "test.pdf",
    "fichierTaille": 49415,
    "categoriesFamille": "DOCUMENTS ENTRANTS",
    "categoriesCote": "AUTRES",
    "categoriesTypeDocument": "DIVERS",
    "canalId": 1
  });

  // do web service post

  var requestURL2 = "https://api-ged-intra.int.maf.local/v2/FinalizeUpload";
  var url2 = new URL(requestURL2);
  var httpConn2 = url2.openConnection();
  var fieldName2 = "";

  httpConn2.setUseCaches(false);
  httpConn2.setDoOutput(true); // indicates POST method
  httpConn2.setDoInput(true);
  httpConn2.setRequestProperty("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true");
  httpConn2.setRequestProperty("Accept", "application/json;odata.metadata=minimal;odata.streaming=true");
  //httpConn2.setRequestProperty("Accept", "*/*");
  httpConn2.setRequestMethod("POST");
  httpConn2.setConnectTimeout(1000);

  // write the json and get response code
  var os = new OutputStreamWriter(httpConn2.getOutputStream());

  os.write(strJson, 0, strJson.length);
  os.flush();
  var msg2 = httpConn2.getResponseMessage();
  var status2 = httpConn2.getResponseCode();

  if (status2 == 200) {
    log.error("POST FinalizeUpload response OK");
    log.error("Mess " + msg2);
    uploadFile.delete();
  }
  else {
    log.error("Error in POST FinalizeUpload API: " + status2 + " " + httpConn2.getResponseMessage());
    throw new Exception();
  }
}
