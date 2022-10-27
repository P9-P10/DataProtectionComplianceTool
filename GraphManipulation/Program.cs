// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using Dapper;

var cs = "Data Source=/home/ane/Documents/GitHub/Legeplads/Databases/SimpleDatabase.sqlite";

using var conn = new SQLiteConnection(cs);
conn.Open();

// var command = conn.CreateCommand();
// command.CommandText = "SELECT * FROM sqlite_master;";

var result = conn.Query("SELECT * FROM sqlite_master AS m JOIN pragma_table_list(m.name);").ToList();

result.ForEach(Console.WriteLine);
