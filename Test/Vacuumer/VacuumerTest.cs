﻿using System;
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
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();


        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(tableColumnPairs);
        List<string> query = vacuumer.GenerateSelectStatementForDataToDelete();
        
        Assert.Empty(query);
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs()
    {
        Purpose purpose = new("Name", "2y", "Condition", "Local", true);
        TableColumnPair tableColumnPair1 = new("Table", "Column");
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();
        tableColumnPair1.AddPurpose(purpose);
        tableColumnPairs.Add(tableColumnPair1);


        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(tableColumnPairs);
        List<string> query = vacuumer.GenerateSelectStatementForDataToDelete(expectedTime);


        string expected =
            $"SELECT Column FROM Table WHERE (Condition);";
        Assert.Contains(expected, query);
    }

    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_Purposes_One_TableColumnPair()
    {
        Purpose purpose = new("Name", "2y", "Condition", "Local", true);
        Purpose purpose2 = new("Second", "3y", "SecondCondition", "Local", true);
        TableColumnPair tableColumnPair1 = new("Table", "Column");
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();
        tableColumnPair1.AddPurpose(purpose);
        tableColumnPair1.AddPurpose(purpose2);
        tableColumnPairs.Add(tableColumnPair1);


        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(tableColumnPairs);
        List<string> query = vacuumer.GenerateSelectStatementForDataToDelete(expectedTime);


        string expected =
            $"SELECT Column FROM Table WHERE (Condition) AND (SecondCondition);";
        Assert.Contains(expected, query);
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        Purpose purpose = new("Name", "2y", "Condition", "Local", true);
        TableColumnPair tableColumnPair1 = new("Table", "Column");
        TableColumnPair tableColumnPair2 = new("SecondTable", "SecondColumn");
        List<TableColumnPair> tableColumnPairs = new List<TableColumnPair>();
        tableColumnPair1.AddPurpose(purpose);
        tableColumnPair2.AddPurpose(purpose);
        tableColumnPairs.Add(tableColumnPair1);
        tableColumnPairs.Add(tableColumnPair2);


        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        GraphManipulation.Vacuuming.Vacuumer vacuumer = new(tableColumnPairs);
        List<string> query = vacuumer.GenerateSelectStatementForDataToDelete(expectedTime);


        string firstExpected =
            $"SELECT Column FROM Table WHERE (Condition);";
        string secondExpected = "SELECT SecondColumn FROM SecondTable WHERE (Condition);";
        Assert.Contains(firstExpected, query);
        Assert.Contains(secondExpected, query);
    }
}