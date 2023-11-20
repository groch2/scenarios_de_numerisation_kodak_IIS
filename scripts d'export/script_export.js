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
  const document = context.getReleaseItem();
  const fileId = document.getField("file_id").value;
  const file = new File("c:\\InfoInputSolution\\exports\\" + fileId + ".pdf");

  if (!file.exists()) {
    var errorMessage = "le fichier à exporter n'existe pas";
    log.error(errorMessage);
    throw new Exception(errorMessage);
  }

  if (!file.canRead()) {
    var errorMessage = "le fichier à exporter ne peut pas être lu";
    log.error(errorMessage);
    throw new Exception(errorMessage);
  }

  const gedApiBaseAddress = "https://api-ged-intra.int.maf.local/v2/";

  const requestURL_1 = gedApiBaseAddress + "upload";
  const urlConnection_1 = new URL(requestURL_1).openConnection();
  out.println("url connection opened: " + !!urlConnection_1);

  urlConnection_1.setUseCaches(false);
  urlConnection_1.setDoOutput(true);
  urlConnection_1.setDoInput(true);
  const boundary = System.currentTimeMillis().toString();
  urlConnection_1.setRequestProperty("Content-Type", "multipart/form-data;boundary=" + boundary);
  urlConnection_1.setRequestProperty("Accept", "application/json; charset=utf-8");
  urlConnection_1.setRequestMethod("POST");
  urlConnection_1.setConnectTimeout(1000);

  const outputStream_1 = urlConnection_1.getOutputStream();
  const outputStreamWriter_1 = new OutputStreamWriter(outputStream_1, "utf-8")
  const printWriter_1 = new PrintWriter(outputStreamWriter_1, true);

  printWriter_1.append("--" + boundary).append("\r\n");
  const fileName = file.getName();
  printWriter_1.append(
    "Content-Disposition: form-data; name=file; filename=\"" + fileName + "\"")
    .append("\r\n");

  printWriter_1
    .append("Content-Type: application/pdf")
    .append("\r\n")
    .append("\r\n");
  printWriter_1.flush();

  const fileInputStream_1 = new FileInputStream(file);
  const buffer = java.lang.reflect.Array.newInstance(java.lang.Byte.TYPE, 4096)
  var bytesRead = -1;
  while ((bytesRead = fileInputStream_1.read(buffer)) !== -1) {
    outputStream_1.write(buffer, 0, bytesRead);
  }
  outputStream_1.flush();
  fileInputStream_1.close();

  printWriter_1.append("\r\n");
  printWriter_1.flush();

  printWriter_1.append("\r\n").flush();
  printWriter_1.append("--" + boundary + "--").append("\r\n");
  printWriter_1.close();
  outputStreamWriter_1.close();
  outputStream_1.close();

  const status_1 = urlConnection_1.getResponseCode();
  if (status_1 !== 200) {
    var error = JSON.stringify({
      workflowStep: "file upload",
      httpStatus: status_1,
      httpResponseMessage: urlConnection_1.getResponseMessage()
    })
    log.error(error);
    throw new Exception(error);
  }

  const inputStream_1 = urlConnection_1.getInputStream();
  const inputStreamReader_1 = new InputStreamReader(inputStream_1)
  const bufferedReader_1 = new BufferedReader(inputStreamReader_1);
  const stringBuilder_1 = new StringBuilder();
  var line = "";
  while ((line = bufferedReader_1.readLine()) !== null) {
    stringBuilder_1.append(line + "\n");
  }
  bufferedReader_1.close();
  inputStreamReader_1.close();
  inputStream_1.close();
  urlConnection_1.disconnect();
  const guidFile = JSON.parse(stringBuilder_1.toString()).guidFile;

  const documentFields = document.getFields();
  const canal_id = documentFields["canal_id"].getValue();
  const depose_par = documentFields["depose_par"].getValue();
  const date_document = documentFields["date_document"].getValue();
  const libelle = documentFields["libelle"].getValue();
  const nom_fichier = documentFields["nom_fichier"].getValue();
  const famille_code = documentFields["famille_code"].getValue();
  const cote_code = documentFields["cote_code"].getValue();
  const type_document_code = documentFields["type_document_code"].getValue();
  const fileSize = file.length();
  const jsonDocumentMetadata = JSON.stringify({
    "fileId": guidFile,
    "libelle": libelle,
    "deposePar": depose_par,
    "dateDocument": date_document,
    "fichierNom": nom_fichier,
    "fichierTaille": fileSize,
    "categoriesFamille": famille_code,
    "categoriesCote": cote_code,
    "categoriesTypeDocument": type_document_code,
    "canalId": canal_id
  });

  const requestURL_2 = gedApiBaseAddress + "FinalizeUpload";
  const urlConnection_2 = new URL(requestURL_2).openConnection();
  urlConnection_2.setUseCaches(false);
  urlConnection_2.setDoOutput(true);
  urlConnection_2.setDoInput(true);
  urlConnection_2.setRequestProperty("Content-Type", "application/json; charset=utf-8");
  urlConnection_2.setRequestProperty("Accept", "application/json; charset=utf-8");
  urlConnection_2.setRequestMethod("POST");
  urlConnection_2.setConnectTimeout(1000);

  const outputStream_2 = urlConnection_2.getOutputStream()
  const outputStreamWriter_2 = new OutputStreamWriter(outputStream_2);
  outputStreamWriter_2.write(jsonDocumentMetadata, 0, jsonDocumentMetadata.length);
  outputStreamWriter_2.flush();
  outputStreamWriter_2.close();
  outputStream_2.close();
  const status_2 = urlConnection_2.getResponseCode();
  if (status_2 === 200) {
    file.delete();
  }
  else {
    var error = JSON.stringify({
      workflowStep: "finalize upload",
      httpStatus: status_2,
      httpResponseMessage: urlConnection_2.getResponseMessage()
    })
    log.error(error);
    throw new Exception(error);
  }
  urlConnection_2.disconnect();
}
