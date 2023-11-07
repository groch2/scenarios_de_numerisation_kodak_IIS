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
var uploadId = GetGuid();
var libelle = GetLetterStartingGuid();

var documentUpload = new {
  fileId = uploadId,
  libelle,
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
var jsonString = await documentUploadJson.ReadAsStringAsync();
JsonDocument.Parse(jsonString).Dump();
SortPropertiesAlphabetically(jsonString).Dump();

static JsonObject SortPropertiesAlphabetically(string jsonString) {
    var jsonObject = JsonDocument.Parse(jsonString).RootElement;
    var sortedProperties = new SortedDictionary<string, JsonElement>(StringComparer.InvariantCultureIgnoreCase);
    foreach (var property in jsonObject.EnumerateObject()) {
        sortedProperties.Add(property.Name, property.Value);
    }
    var sortedJsonObject = new JsonObject();
    foreach (var property in sortedProperties) {
        sortedJsonObject.Add(property.Key, JsonValue.Create<JsonElement>(property.Value));
    }
    return sortedJsonObject;
}

static string GetGuid() => Guid.NewGuid().ToString("N").ToUpperInvariant();

static string GetLetterStartingGuid() {
	var guid = GetGuid();
	var firstLetterPosition = Regex.Match(guid, @"[a-z]", RegexOptions.IgnoreCase).Index;
	guid = $"{guid.Substring(firstLetterPosition)}{guid.Substring(0, firstLetterPosition)}";
	return guid;
}