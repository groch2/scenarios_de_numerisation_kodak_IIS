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
  out.println("is log available in export script in context function: " + (!!log));

  out.println("début de l'envoi du document vers MAF GED");
  /*
canal_id
cote
depose_par
date_document
libelle
nom_fichier
famille
type_document
  */

  const document = context.getReleaseItem();
  const documentId = document.getId();
  out.println("documentId: " + documentId);

  const fileId = document.getField("file_id").value;
  out.println("fileId: " + fileId);

  const file = new File("c:\\InfoInputSolution\\exports\\" + fileId + ".pdf");
  out.println("file path: " + file.getPath());
  out.println("file exists: " + file.exists());
  out.println("can read file: " + file.canRead());
  const fileSize = file.length();
  out.println("fileSize : " + fileSize);

  // do web service post
  const requestURL_1 = "https://api-ged-intra.int.maf.local/v2/upload";
  const urlConnection_1 = new URL(requestURL_1).openConnection();
  out.println("url connection opened: " + !!urlConnection_1);

  urlConnection_1.setUseCaches(false);
  urlConnection_1.setDoOutput(true);
  urlConnection_1.setDoInput(true);
  const boundary = System.currentTimeMillis().toString();
  out.println("boundary: " + boundary);
  urlConnection_1.setRequestProperty("Content-Type", "multipart/form-data; boundary=" + boundary);
  urlConnection_1.setRequestProperty("Accept", "application/json; utf-8");
  urlConnection_1.setRequestProperty("Accept", "*/*");
  urlConnection_1.setRequestMethod("POST");
  urlConnection_1.setConnectTimeout(1000);
  out.println("urlConnection configured: " + !!urlConnection_1);

  const fileName = file.getName();
  const outputStream_1 = urlConnection_1.getOutputStream();
  const printWriter_1 = new PrintWriter(new OutputStreamWriter(outputStream_1, "utf-8"), true);
  out.println("initialisation de la préparation de la requête http terminée")

  printWriter_1.append("--" + boundary).append("\r\n");
  out.println("PrintWriter fonctionnne");

  printWriter_1.append(
    "Content-Disposition: form-data; name=\"\"; filename=\"" + fileName + "\"")
    .append("\r\n");

  printWriter_1.append(
    "Content-Type: application/pdf")
    .append("\r\n");

  printWriter_1.append("Content-Transfer-Encoding: binary").append("\r\n");
  printWriter_1.append("\r\n");
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

  out.println("envoi de la requête http d'upload du document");
  const status = urlConnection_1.getResponseCode();
  if (status !== 200) {
    out.println("Error in POST Upload API: " + status + " " + urlConnection_1.getResponseMessage());
    throw new Exception();
  }

  out.println("POST Upload response OK");
  const msg = urlConnection_1.getResponseMessage();
  out.println("Mess " + msg);

  const bufferedReader_1 = new BufferedReader(new InputStreamReader(urlConnection_1.getInputStream()));
  const stringBuilder_1 = new StringBuilder();
  var line = "";
  while ((line = bufferedReader_1.readLine()) !== null) {
    stringBuilder_1.append(line + "\n");
  }
  bufferedReader_1.close();
  urlConnection_1.disconnect();
  out.println("identifiant de l'upload du document: " + stringBuilder_1.toString());
  const json = JSON.parse(stringBuilder_1.toString())
  out.println("guidFile : " + json.guidFile);

  const documentFields = document.getFields();
  const canal_id = documentFields["canal_id"].getValue();
  const depose_par = documentFields["depose_par"].getValue();
  const date_document = documentFields["date_document"].getValue();
  const libelle = documentFields["libelle"].getValue();
  out.println("libellé: " + libelle);
  const nom_fichier = documentFields["nom_fichier"].getValue();
  const famille_code = documentFields["famille_code"].getValue();
  const cote_code = documentFields["cote_code"].getValue();
  const type_document_code = documentFields["type_document_code"].getValue();
  const jsonDocumentMetadata = JSON.stringify({
    "fileId": json.guidFile,
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
  out.println("jsonDocumentMetadata: " + jsonDocumentMetadata);

  // do web service post
  const requestURL_2 = "https://api-ged-intra.int.maf.local/v2/FinalizeUpload";
  const urlConnection_2 = new URL(requestURL_2).openConnection();
  urlConnection_2.setUseCaches(false);
  urlConnection_2.setDoOutput(true);
  urlConnection_2.setDoInput(true);
  urlConnection_2.setRequestProperty("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true");
  urlConnection_2.setRequestProperty("Accept", "application/json;odata.metadata=minimal;odata.streaming=true");
  urlConnection_2.setRequestMethod("POST");
  urlConnection_2.setConnectTimeout(1000);
  out.println("initialisation de la requête de finalisation OK");

  // write the json and get response code
  const outputStreamWriter_2 = new OutputStreamWriter(urlConnection_2.getOutputStream());
  outputStreamWriter_2.write(jsonDocumentMetadata, 0, jsonDocumentMetadata.length);
  outputStreamWriter_2.flush();
  const status_2 = urlConnection_2.getResponseCode();
  out.println("statut de la réponse de la requête de finalisation: " + status_2);
  if (status_2 === 200) {
    out.println("POST FinalizeUpload response OK");
    const msg_2 = urlConnection_2.getResponseMessage();
    out.println("Mess " + msg_2);
    file.delete();
  }
  else {
    out.println("Error in POST FinalizeUpload API: " + status_2 + " " + urlConnection_2.getResponseMessage());
    throw new Exception();
  }
  out.println("l'export est terminé avec succès");
  urlConnection_2.disconnect();
}
