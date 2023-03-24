using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using GraphManipulation.Vacuuming.Components;
using Moq;
using Xunit;

namespace Test.Vacuumer;

public class VacuumerTest
{
    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Empty_Query_when_No_TablePairs_Provided()
    {
        var dbMock = new Mock<IDbConnection>();

        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();


        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(dbMock.Object, tableColumnPairs);
        string query = vacuumer.GenerateSqlQueryForDeletion();


        string expected = "";
        Assert.Equal(query, expected);
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs()
    {
        var dbMock = new Mock<IDbConnection>();

        Purpose purpose = new("Name", "2y", "Condition", "Local", true);
        TableColumnPair tableColumnPair1 = new("Table", "Column");
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();
        tableColumnPair1.AddPurpose(purpose);
        tableColumnPairs.Add(tableColumnPair1);


        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(dbMock.Object, tableColumnPairs);
        string query = vacuumer.GenerateSqlQueryForDeletion(expectedTime);


        string expected =
            $"SELECT Column, expiration_date FROM Table JOIN (Condition) WHERE expiration_date < {expectedTime} AND uid = id;\n";
        Assert.Equal(query, expected);
    }
    
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_Purposes_One_TableColumnPair()
    {
        var dbMock = new Mock<IDbConnection>();

        Purpose purpose = new("Name", "2y", "Condition", "Local", true);
        Purpose purpose2 = new("Second", "2y", "SecondCondition", "Local", true);
        TableColumnPair tableColumnPair1 = new("Table", "Column");
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();
        tableColumnPair1.AddPurpose(purpose);
        tableColumnPair1.AddPurpose(purpose2);
        tableColumnPairs.Add(tableColumnPair1);
        


        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(dbMock.Object, tableColumnPairs);
        string query = vacuumer.GenerateSqlQueryForDeletion(expectedTime);


        string expected =
            $"SELECT Column, expiration_date FROM Table JOIN (Condition) WHERE expiration_date < {expectedTime} AND uid = id;\n" +
            $"SELECT Column, expiration_date FROM Table JOIN (SecondCondition) WHERE expiration_date < {expectedTime} AND uid = id;\n";
        Assert.Equal(query, expected);
    }
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        var dbMock = new Mock<IDbConnection>();

        Purpose purpose = new("Name", "2y", "Condition", "Local", true);
        TableColumnPair tableColumnPair1 = new("Table", "Column");
        TableColumnPair tableColumnPair2 = new("SecondTable", "SecondColumn");
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();
        tableColumnPair1.AddPurpose(purpose);
        tableColumnPair2.AddPurpose(purpose);
        tableColumnPairs.Add(tableColumnPair1);
        


        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(dbMock.Object, tableColumnPairs);
        string query = vacuumer.GenerateSqlQueryForDeletion(expectedTime);


        string expected =
            $"SELECT Column, expiration_date FROM Table JOIN (Condition) WHERE expiration_date < {expectedTime} AND uid = id;\n" +
            $"SELECT SecondColumn, expiration_date FROM SecondTable JOIN (Condition) WHERE expiration_date < {expectedTime} AND uid = id;\n";
        Assert.Equal(query, expected);
    }
}