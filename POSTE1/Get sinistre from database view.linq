<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

using var connection =
	new SqlConnection("Server=bdd-ged.int.maf.local;Database=GEDMAF;Trusted_Connection=True;");
using var command = connection.CreateCommand();
command.CommandText =
	"SELECT TOP 1 [Numero_Sinistre], [NO_DOSSIER], [Groupe], [numero_Adherent], [compte_nt] FROM [dbo].[V_AFFAIRE_PAPS]";
using var dataAdapter = new SqlDataAdapter(command);
var dataTable = new DataTable();
connection.Open();
dataAdapter.Fill(dataTable);
dataTable.Dump();