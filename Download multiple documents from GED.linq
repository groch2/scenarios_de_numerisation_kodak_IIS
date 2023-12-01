<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
	const string downloadDirectory =
		@"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO1\contrats signés";
	var downloadDocuments =
		File
			.ReadAllLines(@"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\Spécifications des scénarios de numérisation\GECO1\Identifiants GED de documents à numériser.txt")
			.Select(documentGedId => DownloadDocumentByDocumentId(documentGedId, downloadDirectory))
			.ToArray();
	await Task.WhenAll(downloadDocuments);
	var séparateurDePli = new FileInfo(@"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO1\Séparateur de plis GECO1.jpg");
	new DirectoryInfo(downloadDirectory)
		.GetFiles()
		.OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase)
		.Select((file, index) => new { file, index })
		.ToList()
		.ForEach(item => {
			var index = (item.index + 1) * 2 - 1;
			item.file.MoveTo(Path.Combine(downloadDirectory, $"{index:00}-{item.file.Name}"));
			séparateurDePli.CopyTo(Path.Combine(downloadDirectory, $"{index + 1:00}-SEP_PLI.jpg"));
		});
}

const string apiVersion = "v2";
HttpClient client = new HttpClient { BaseAddress = new Uri("https://api-ged-intra.int.maf.local/") };
async Task DownloadDocumentByDocumentId(string documentId, string downloadDirectory) {
	var downloadAddress = new Uri($"/{apiVersion}/download?documentId={documentId}", UriKind.Relative);
	using var downloadRequest = new HttpRequestMessage(HttpMethod.Get, downloadAddress);
	var httpDownloadResponse = await client.SendAsync(downloadRequest);
	var fileName =
		$"{Path.GetFileNameWithoutExtension(await GedDocumentFichierNom(documentId))}.pdf" ??
			$"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.pdf";
	var fileDownloadPath = Path.Combine(downloadDirectory, fileName);
	using var outputFileStream = new FileStream(fileDownloadPath, FileMode.Create);
	using var downloadStream = await httpDownloadResponse.Content.ReadAsStreamAsync();
	await downloadStream.CopyToAsync(outputFileStream);
}

async Task<string> GedDocumentFichierNom(string documentId) {
	var getDocumentFichierNomeAddress =
		new Uri($"/{apiVersion}/Documents/{documentId}?select=fichierNom,libelle", UriKind.Relative);
	var jsonResponse = await client.GetStringAsync(getDocumentFichierNomeAddress);
	var result = JsonDocument.Parse(jsonResponse).RootElement;
	var fichierNom = result.GetProperty("fichierNom").GetString();
	var libelle = result.GetProperty("libelle").GetString();
	return fichierNom ?? libelle;
}