<Query Kind="Statements">
  <Namespace>System.Data.SqlClient</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeLinqToSql>true</IncludeLinqToSql>
</Query>

const string sqlServer = "bdd-donum.int.maf.local";

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
	.Select(columnsName => new { columnsName, isNull = dataTable.Rows[0].IsNull(columnsName) })
	.Where(rowItems => !rowItems.isNull)
	.Select(rowItems => new { column = rowItems.columnsName, value = dataTable.Rows[0][rowItems.columnsName] })
	.OrderBy(rowItems => rowItems.column)
	.Dump();
