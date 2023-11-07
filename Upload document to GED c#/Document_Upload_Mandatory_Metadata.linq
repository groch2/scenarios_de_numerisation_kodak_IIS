<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

const string filePath = @"C:\Users\deschaseauxr\Documents\MAFlyDoc\test.pdf";
var fileName = Path.GetFileName(filePath);
var uploadId = Guid.NewGuid().ToString("N").ToUpperInvariant();

var documentUpload = new {
  fileId = uploadId,
  libelle = fileName,
  deposePar = "ROD",
  dateDocument = DateTime.Now.ToUniversalTime(),
  fichierNom = fileName,
  fichierTaille = new FileInfo(filePath).Length,
  categoriesFamille = "DOCUMENTS ENTRANTS",
  categoriesCote = "AUTRES",
  categoriesTypeDocument = "DIVERS",
  canalId = 1,
};
var documentUploadJson = JsonContent.Create(documentUpload);
var jsonDocument = await documentUploadJson.ReadAsStringAsync();
JsonDocument.Parse(jsonDocument).Dump();
