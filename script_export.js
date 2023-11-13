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
  debug.print("début de l'envoi du document vers MAF GED");
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
  debug.print({ documentId: documentId });
  const batch = document.getBatch();
  const canal_id = batch.fields["canal_id"].value;
  const cote = batch.fields["cote"].value;
  const depose_par = batch.fields["depose_par"].value;
  const date_document = batch.fields["date_document"].value;
  const libelle = batch.fields["libelle"].value;
  const nom_fichier = batch.fields["nom_fichier"].value;
  const famille = batch.fields["famille"].value;
  const type_document = batch.fields["type_document"].value;
  debug.print("données d'indexation");
  debug.print({
    canal_id: canal_id,
    cote: cote,
    depose_par: depose_par,
    date_document: date_document,
    libelle: libelle,
    nom_fichier: nom_fichier,
    famille: famille,
    type_document: type_document
  });

  const docFiles = context.getSharedObject(ImagesReleaseCommon.OUTPARAM_FILES);
  const exportedFile = new File(docFiles[documentId][0]);
  const fileSize = exportedFile.length();

  // do web service post
  const requestURL = "https://api-ged-intra.int.maf.local/v2/upload";
  const urlConnection = new URL(requestURL).openConnection();
  const fieldName = "";

  urlConnection.setUseCaches(false);
  urlConnection.setDoInput(true);
  urlConnection.setRequestProperty("Content-Type", "multipart/form-data; boundary=" + boundary);
  urlConnection.setRequestProperty("Accept", "application/json; utf-8");
  urlConnection.setRequestProperty("Accept", "*/*");
  urlConnection.setRequestMethod("POST");
  urlConnection.setConnectTimeout(1000);

  debug.print("docFiles: " + docFiles);
  const uploadFile = new File(docFiles[documentId][0]);
  const fileName = uploadFile.getName();
  const outputStream = urlConnection.getOutputStream();
  const writer = new PrintWriter(new OutputStreamWriter(outputStream, "utf-8"), true);

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
  const inputStream = new FileInputStream(uploadFile);

  const buffer = java.lang.reflect.Array.newInstance(java.lang.Byte.TYPE, 4096)
  const bytesRead = -1;
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

  const status = urlConnection.getResponseCode();
  if (status != 200) {
    debug.print("Error in POST Upload API: " + status + " " + urlConnection.getResponseMessage());
    throw new Exception();
  }

  debug.print("POST Upload response OK");
  const msg = urlConnection.getResponseMessage();
  debug.print("Mess " + msg);

  const br = new BufferedReader(new InputStreamReader(urlConnection.getInputStream()));
  const sb = new StringBuilder();
  const line = "";
  while ((line = br.readLine()) != null) {
    sb.append(line + "\n");
  }
  br.close();
  debug.print("GuiID Normalement " + sb.toString());
  const json = JSON.parse(sb.toString())
  debug.print("guidFile : " + json.guidFile);

  // create json
  const strJson = JSON.stringify({
    "fileId": json.guidFile,
    "libelle": libelle,
    "deposePar": depose_par,
    "dateDocument": date_document,
    "fichierNom": nom_fichier,
    "fichierTaille": fileSize,
    "categoriesFamille": famille,
    "categoriesCote": cote,
    "categoriesTypeDocument": type_document,
    "canalId": canal_id
  });

  // do web service post
  const requestURL2 = "https://api-ged-intra.int.maf.local/v2/FinalizeUpload";
  const urlConnection2 = new URL(requestURL2).openConnection();
  urlConnection2.setUseCaches(false);
  urlConnection2.setDoInput(true);
  urlConnection2.setRequestProperty("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true");
  urlConnection2.setRequestProperty("Accept", "application/json;odata.metadata=minimal;odata.streaming=true");
  urlConnection2.setRequestMethod("POST");
  urlConnection2.setConnectTimeout(1000);

  // write the json and get response code
  const os = new OutputStreamWriter(urlConnection2.getOutputStream());
  os.write(strJson, 0, strJson.length);
  os.flush();
  const status2 = urlConnection2.getResponseCode();
  if (status2 == 200) {
    debug.print("POST FinalizeUpload response OK");
    const msg2 = urlConnection2.getResponseMessage();
    debug.print("Mess " + msg2);
    uploadFile.delete();
  }
  else {
    debug.print("Error in POST FinalizeUpload API: " + status2 + " " + urlConnection2.getResponseMessage());
    throw new Exception();
  }
}
