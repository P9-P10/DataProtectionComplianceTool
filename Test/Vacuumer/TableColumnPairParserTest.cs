using System.Data.SQLite;
using Dapper;
using GraphManipulation.Vacuuming;
using GraphManipulation.Vacuuming.Components;
using Xunit;

namespace Test.Vacuumer;

public class TableColumnPairParserTest
{
    private SQLiteConnection Setup()
    {
        SQLiteConnection.CreateFile("testSqlite.sqlite");
        var connectionString = "Data Source=MyDatabase.sqlite;Version=3;";
        SQLiteConnection sqLiteConnection = new(connectionString);
        sqLiteConnection.Execute("DROP TABLE IF EXISTS gdpr_metadata;");
        sqLiteConnection.Execute("CREATE TABLE IF NOT EXISTS gdpr_metadata(" +
                                 "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                 "purpose VARCHAR(256), ttl VARCHAR(256),  " +
                                 "target_table VARCHAR(256), " +
                                 "target_column VARCHAR(256), " +
                                 "origin VARCHAR(256), " +
                                 "start_time VARCHAR(256), " +
                                 "legally_required VARCHAR(256)); ");

        sqLiteConnection.Execute("INSERT INTO gdpr_metadata " +
                                 "(id,purpose,ttl,target_table,target_column,origin,start_time,legally_required) " +
                                 "VALUES (1, " +
                                 "'Marketing', " +
                                 "'2y', " +
                                 "'newsletter', " +
                                 "'email', " +
                                 "'local', " +
                                 "'Condition', " +
                                 "False)");

        return sqLiteConnection;
    }

    [Fact]
    public void TestFetchTableColumnPairs_Fetches_Correct_Purpose_Count()
    {
        var sqLiteConnection = Setup();

        TableColumnPairParser tableColumnPairParser = new(sqLiteConnection);
        var result = tableColumnPairParser.FetchTableColumnPairs();

        Assert.True(result.Count == 1);
    }

    [Fact]
    public void TestFetchTableColumnPairs_Fetches_Correct_TC_Pair()
    {
        var sqLiteConnection = Setup();

        TableColumnPairParser tableColumnPairParser = new(sqLiteConnection);
        var result = tableColumnPairParser.FetchTableColumnPairs();
        TableColumnPair tableColumnPair = new("newsletter", "email");
        Purpose purpose = new("Marketing", "2y", "Condition", "local", false);


        Assert.Contains(tableColumnPair, result);
        var firstElement = result[0];
        Assert.Contains(purpose, firstElement.GetPurposes);
    }

    [Fact]
    public void TestTableColumnPairs_Merges_Equivalent_TcPairs_Into_One()
    {
        var sqLiteConnection = Setup();
        sqLiteConnection.Execute("INSERT INTO gdpr_metadata " +
                                 "(id,purpose,ttl,target_table,target_column,origin,start_time,legally_required) " +
                                 "VALUES (2, " +
                                 "'Bookkeeping', " +
                                 "'5y', " +
                                 "'newsletter', " +
                                 "'email', " +
                                 "'local', " +
                                 "'Condition', " +
                                 "True)");

        TableColumnPairParser tableColumnPairParser = new(sqLiteConnection);
        var result = tableColumnPairParser.FetchTableColumnPairs();
        TableColumnPair tableColumnPair = new("newsletter", "email");
        Purpose purpose = new("Marketing", "2y", "Condition", "local", false);
        Purpose secondPurpose = new("Bookkeeping", "5y", "Condition", "local", true);


        Assert.Contains(tableColumnPair, result);
        var firstElement = result[0];
        Assert.Contains(purpose, firstElement.GetPurposes);
        Assert.Contains(secondPurpose, firstElement.GetPurposes);
    }

    [Fact]
    public void TestTableColumnPairs_Returns_Multiple_TcPairs()
    {
        var sqLiteConnection = Setup();
        sqLiteConnection.Execute("INSERT INTO gdpr_metadata " +
                                 "(id,purpose,ttl,target_table,target_column,origin,start_time,legally_required) " +
                                 "VALUES (2, " +
                                 "'Bookkeeping', " +
                                 "'5y', " +
                                 "'newsletter', " +
                                 "'id', " +
                                 "'local', " +
                                 "'Condition', " +
                                 "True)");

        TableColumnPairParser tableColumnPairParser = new(sqLiteConnection);
        var result = tableColumnPairParser.FetchTableColumnPairs();
        TableColumnPair tableColumnPair = new("newsletter", "email");
        var secondTableColumnPair = new TableColumnPair("newsletter", "id");
        Purpose purpose = new("Marketing", "2y", "Condition", "local", false);
        Purpose secondPurpose = new("Bookkeeping", "5y", "Condition", "local", true);


        Assert.Contains(tableColumnPair, result);
        Assert.Contains(secondTableColumnPair, result);
        var firstElement = result[0];
        var secondElement = result[1];
        Assert.Contains(purpose, firstElement.GetPurposes);
        Assert.Contains(secondPurpose, secondElement.GetPurposes);
    }
}