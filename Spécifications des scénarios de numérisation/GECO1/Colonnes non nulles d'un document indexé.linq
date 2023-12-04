<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string sqlServer = "bdd-donum.int.maf.local";

var gedFieldsByFormerName =
	File
		.ReadAllLines(@"C:\Users\deschaseauxr\Documents\Kodak Info Input Solution\Mapping_document_archeamaf.txt")
		.Select(line => line.Split("\t"))
		.ToDictionary(pair => pair[0], pair => pair[1], StringComparer.OrdinalIgnoreCase);
using var connection = new SqlConnection($"Server={sqlServer};Database=gedmaf;Integrated Security=True");
connection.Open();
using var command = connection.CreateCommand();
command.CommandText = @"select top 1 *
from ARCHEAMAF
where CODE_PIECE='GECO1'
and LIB_DOC like '%signé'
order by docn desc";
var dataTable = new DataTable();
var dataAdapter = new SqlDataAdapter(command);
dataAdapter.Fill(dataTable);
var columnsNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.Caption).ToArray();
columnsNames
	.Select(columnsName => {
		gedFieldsByFormerName.TryGetValue(columnsName, out var gedField);
		return new { columnsName, gedField, isNull = dataTable.Rows[0].IsNull(columnsName) };
	})
	.Where(rowItems => !rowItems.isNull)
	.Select(rowItems => new { column = rowItems.columnsName, gedField = rowItems.gedField, value = dataTable.Rows[0][rowItems.columnsName] })
	.OrderBy(rowItems => rowItems.column)
	.Dump();
