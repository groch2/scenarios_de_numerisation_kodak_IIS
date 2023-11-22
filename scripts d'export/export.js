importPackage(Packages.scanserver.released.rdpi);
importPackage(Packages.scancommon.model.release);
importPackage(Packages.com.imagetrust.tc.model.object);
importPackage(javax.net.ssl);

importClass(java.io.BufferedReader);
importClass(java.io.File);
importClass(java.io.FileInputStream);
importClass(java.io.InputStreamReader);
importClass(java.io.OutputStreamWriter);
importClass(java.io.PrintWriter);
importClass(java.lang.StringBuilder);
importClass(java.net.URL);

function release(context) {
  const gedApiBaseAddress = "https://api-ged-intra.int.maf.local/v2/";

  (function () {
    const document = context.getReleaseItem();
    const file = (function () {
      const releaseItemId = document.getId();
      const outputFiles = context.getSharedObject(ImagesReleaseCommon.OUTPARAM_FILES)[releaseItemId];
      return new File(outputFiles[0]);
    })();

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

    const fileUploadGuid = uploadDocumentToGED(file).fileUploadGuid;
    out.println(JSON.stringify({ fileUploadGuid: fileUploadGuid }));
    log.info(JSON.stringify({ fileUploadGuid: fileUploadGuid }));

    const documentId =
      finalizeDocumentUpload({
        fileUploadGuid: fileUploadGuid,
        documentFields: document.getFields(),
        fileSize: file.length()
      }).documentId;
    file.delete();
    out.println(JSON.stringify({ documentId: documentId }));
    log.info(JSON.stringify({ documentId: documentId }));
  })();

  function uploadDocumentToGED(file) {
    const requestURL = gedApiBaseAddress + "upload";
    const urlConnection = new URL(requestURL).openConnection();

    urlConnection.setUseCaches(false);
    urlConnection.setDoOutput(true);
    urlConnection.setDoInput(true);
    const boundary = System.currentTimeMillis().toString();
    urlConnection.setRequestProperty("Content-Type", "multipart/form-data;boundary=" + boundary);
    urlConnection.setRequestProperty("Accept", "application/json; charset=utf-8");
    urlConnection.setRequestMethod("POST");
    urlConnection.setConnectTimeout(1000);
    urlConnection.connect();

    const outputStream = urlConnection.getOutputStream();
    const outputStreamWriter = new OutputStreamWriter(outputStream, "utf-8")
    const printWriter = new PrintWriter(outputStreamWriter, true);

    printWriter.append("--" + boundary).append("\r\n");
    const fileName = file.getName();
    printWriter.append(
      "Content-Disposition: form-data; name=file; filename=\"" + fileName + "\"")
      .append("\r\n");

    printWriter
      .append("Content-Type: application/pdf")
      .append("\r\n")
      .append("\r\n");
    printWriter.flush();

    const fileInputStream = new FileInputStream(file);
    const buffer = java.lang.reflect.Array.newInstance(java.lang.Byte.TYPE, 4096)
    var bytesRead = -1;
    while ((bytesRead = fileInputStream.read(buffer)) !== -1) {
      outputStream.write(buffer, 0, bytesRead);
    }
    outputStream.flush();
    fileInputStream.close();

    printWriter.append("\r\n");
    printWriter.flush();

    printWriter.append("\r\n").flush();
    printWriter.append("--" + boundary + "--").append("\r\n");
    printWriter.close();
    outputStreamWriter.close();
    outputStream.close();

    const responseCode = urlConnection.getResponseCode();
    if (responseCode !== 200) {
      var error = JSON.stringify({
        workflowStep: "file upload",
        httpStatus: responseCode,
        httpResponseMessage: urlConnection.getResponseMessage()
      })
      urlConnection.disconnect();
      log.error(error);
      out.println(error);
      throw new Exception(error);
    }

    const responseContent = getHttpRequestResponseContent(urlConnection);
    urlConnection.disconnect();

    return { fileUploadGuid: JSON.parse(responseContent).guidFile };
  }

  function finalizeDocumentUpload({ fileUploadGuid: fileUploadGuid, documentFields: documentFields, fileSize: fileSize }) {
    const canal_id = documentFields["canal_id"].getValue();
    const depose_par = documentFields["depose_par"].getValue();
    const date_document = documentFields["date_document"].getValue();
    const libelle = documentFields["libelle"].getValue();
    const nom_fichier = documentFields["nom_fichier"].getValue();
    const famille_code = documentFields["famille_code"].getValue();
    const cote_code = documentFields["cote_code"].getValue();
    const type_document_code = documentFields["type_document_code"].getValue();
    const jsonDocumentMetadata = JSON.stringify({
      "fileId": fileUploadGuid,
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
    out.println("jsonDocumentMetadata :");
    out.println(jsonDocumentMetadata);

    const requestURL = gedApiBaseAddress + "FinalizeUpload";
    const urlConnection = new URL(requestURL).openConnection();
    urlConnection.setUseCaches(false);
    urlConnection.setDoOutput(true);
    urlConnection.setDoInput(true);
    urlConnection.setRequestProperty("Content-Type", "application/json; charset=utf-8");
    urlConnection.setRequestProperty("Accept", "application/json; charset=utf-8");
    urlConnection.setRequestMethod("POST");
    urlConnection.setConnectTimeout(1000);
    urlConnection.connect();

    const outputStream = urlConnection.getOutputStream()
    const outputStreamWriter = new OutputStreamWriter(outputStream);
    outputStreamWriter.write(jsonDocumentMetadata, 0, jsonDocumentMetadata.length);
    outputStreamWriter.flush();
    outputStreamWriter.close();
    outputStream.close();
    const responseCode = urlConnection.getResponseCode();
    if (responseCode === 200) {
      const responseContent = getHttpRequestResponseContent(urlConnection);
      const documentId = JSON.parse(responseContent).documentId
      urlConnection.disconnect();
      return { documentId: documentId };
    }
    const error = JSON.stringify({
      workflowStep: "finalize upload",
      httpStatus: responseCode,
      httpResponseMessage: urlConnection.getResponseMessage()
    })
    urlConnection.disconnect();
    out.println(error);
    log.error(error);
    throw new Exception(error);
  }

  function getHttpRequestResponseContent(urlConnection) {
    const inputStream = urlConnection.getInputStream();
    const inputStreamReader = new InputStreamReader(inputStream)
    const bufferedReader = new BufferedReader(inputStreamReader);
    const stringBuilder = new StringBuilder();
    var line = "";
    while ((line = bufferedReader.readLine()) !== null) {
      stringBuilder.append(line + "\n");
    }
    bufferedReader.close();
    inputStreamReader.close();
    inputStream.close();
    return stringBuilder.toString();
  }
}
