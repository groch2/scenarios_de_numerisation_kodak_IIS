<Query Kind="Statements">
  <Reference>C:\TeamProjects\GED API\MAF.GED.API.Host\bin\Debug\net6.0\MAF.GED.Domain.Model.dll</Reference>
  <Reference Relative="..\Json130r3\Bin\net6.0\Newtonsoft.Json.dll">&lt;MyDocuments&gt;\Json130r3\Bin\net6.0\Newtonsoft.Json.dll</Reference>
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

var httpClient =
	new HttpClient {
		BaseAddress = new Uri("https://api-ged-intra.int.maf.local/v2/Documents/")
	};
var documentIdList =
	File
		.ReadAllLines(@"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\GECO1\documentIdList.txt")
		.Aggregate(
			new StringBuilder("("),
			(state, item) => state.Append($"'{item}',"),
			state => state.Remove(state.Length - 1, 1).Append(')').ToString());
var actual_documents =
	await httpClient.GetStringAsync(
		$"?$filter=documentId in {documentIdList}");
var documents =
	JsonDocument
		.Parse(actual_documents)
		.RootElement
		.GetProperty("value")
		.EnumerateArray()
		.Select(document => Newtonsoft.Json.JsonConvert.DeserializeObject<MAF.GED.Domain.Model.Document>(document.ToString()))
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
