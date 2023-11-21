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
const string libellé = "048EEBC354C749669CBAF2625F9E8633";
var actual_documents =
	await httpClient.GetStringAsync(
		$"?$filter=libelle eq '{libellé}'");
var documents =
	JsonDocument
		.Parse(actual_documents)
		.RootElement
		.GetProperty("value")
		.EnumerateArray()
		.Select(document => Newtonsoft.Json.JsonConvert.DeserializeObject<MAF.GED.Domain.Model.Document>(document.ToString()))
		.Select(document =>
			new {
				document.DeposePar,
				document.CategoriesCote,
				document.CategoriesFamille,
				document.CategoriesTypeDocument,
				document.DateDocument,
				document.DocumentId,
				document.Extension,
				document.FichierNom,
				document.Libelle,
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
