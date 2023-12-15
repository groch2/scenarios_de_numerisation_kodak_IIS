<Query Kind="Statements">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Nodes</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
</Query>

var tempFile = Path.Combine(Path.GetTempPath(), "temporary_KIIS_logs.txt");
File.Copy(
	sourceFileName: @"\\cons-kodak-intra.dev.maf.local\c$\InfoInputSolution\logs\imagetrustreleasedaemon-stdout.2023-12-15.log",
	destFileName: tempFile,
	overwrite: true);
var documentIdList =
	File
		.ReadAllLines(tempFile)
		.Reverse()
		.Where(line => Regex.IsMatch(input: line, pattern: @"\{""documentId"":""\d{26}""\}", options: RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
		.Take(3)
		.Select(
			line =>
				Regex
					.Match(
						input: line,
						pattern: @"\{""documentId"":""(\d{26})""\}",
						options: RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
					.Groups
					.Cast<Group>()
					.ToArray()[1]
					.Value)
		.Aggregate(
			new StringBuilder("("),
			(state, item) => state.Append($"'{item}',"),
			state => state.Remove(state.Length - 1, 1).Append(')').ToString());
var httpClient =
	new HttpClient {
		BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
	};
var actual_documents =
	await httpClient.GetStringAsync(
		$"?$filter=documentId in {documentIdList}");
var jsonSerializerOptions = new JsonSerializerOptions {
	PropertyNameCaseInsensitive = true
};
jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
var documents =
	JsonDocument
		.Parse(actual_documents)
		.RootElement
		.GetProperty("value")
		.EnumerateArray()
		.Select(document => JsonSerializer.Deserialize<MAF.GED.Domain.Model.Document>(document.ToString(), jsonSerializerOptions))
		.Select(document =>
			new {
				document.Libelle,
				document.DocumentId,
				document.CanalPrincipal,
				document.CanalSecondaire,
				document.CategoriesCote,
				document.CategoriesFamille,
				document.CategoriesTypeDocument,
				document.CodeBarreId,
				document.CompteId,
				document.DateDocument,
				document.DateNumerisation,
				document.DeposeLe,
				document.DeposePar,
				document.Docn,
				document.FichierNombrePages,
				document.HeureNumerisation,
				document.Important,
				document.Link,
				document.ModifieLe,
				document.NumeroContrat,
				document.Preview,
				document.QualiteValideeLe,
				document.QualiteValideePar,
				document.QualiteValideeValide,
				document.RegroupementId,
				document.Sens,
				document.Statut,
				document.TraiteLe,
				document.TraitePar,
				document.VuLe,
				document.VuPar,
				queueStatus = GetDocumentStatus(document),
			})
		.OrderBy(document => document.DocumentId);
documents.Dump();

/*
AssigneDepartement
AssigneGroup
AssigneRedacteur
AssureurId
CanalPrincipal
CanalSecondaire
CategoriesCote
CategoriesFamille
CategoriesTypeDocument
ChantierId
CodeBarreId
CodeOrigine
Commentaire
CompteId
DateDocument
DateNumerisation
DeposeLe
DeposePar
Docn
DocumentId
DocumentId
DocumentValide
DuplicationId
Extension
FichierNom
FichierNombrePages
FichierTaille
HeureNumerisation
Horodatage
Important
IsHorsWorkFlowSinapps
Libelle
Link
ModifieLe
ModifiePar
MultiCompteId
Nature
NumeroAvenant
NumeroContrat
NumeroGc
NumeroProposition
NumeroSinistre
PeriodeValiditeDebut
PeriodeValiditeFin
PersonneId
PresenceAr
Preview
PreviewLink
Priorite
Provenance
QualiteValideeLe
QualiteValideePar
QualiteValideeValide
ReferenceAttestation
ReferenceSecondaire
RefTiers
RegroupementId
Sens
SousDossierSinistre
Statut
Tenant
TraiteLe
TraitePar
TypeContact
TypeGarantie
VisibiliteExterne
VisibilitePapsExtranet
VuLe
VuPar
*/

DocumentQueueStatus GetDocumentStatus(MAF.GED.Domain.Model.Document document) {
	return 
		GetDocumentStatus(
		    documentTraiteDate: document.TraiteLe,
		    documentVuDate: document.VuLe,
		    documentQualiteValideeDate: document.QualiteValideeLe,
		    documentIsQualiteValidated: document.QualiteValideeValide);

	DocumentQueueStatus GetDocumentStatus(
	            DateTime? documentTraiteDate,
	            DateTime? documentVuDate,
	            DateTime? documentQualiteValideeDate,
	            bool? documentIsQualiteValidated) =>
	            0 switch {
	                _ when documentTraiteDate != null =>
	                    DocumentQueueStatus.TRAITE,
	                _ when (documentVuDate ?? documentQualiteValideeDate) == null =>
	                    DocumentQueueStatus.NOUVEAU,
	                _ when documentVuDate != null && documentQualiteValideeDate != null && documentIsQualiteValidated != true =>
	                    DocumentQueueStatus.INVALIDE,
	                _ => DocumentQueueStatus.A_TRAITER
	            };
}

enum DocumentQueueStatus {
    NOUVEAU,
    INVALIDE,
    A_TRAITER,
    TRAITE
}
