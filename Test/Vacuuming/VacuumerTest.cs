using System.Collections.Generic;
using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;
using GraphManipulation.Vacuuming;
using Xunit;

namespace Test.Vacuuming;

public class VacuumerTest
{
    private class TestPersonDataColumnService : IPersonDataColumnService
    {
        private readonly List<PersonDataColumn> _personDataColumns = new();

        public void AddColumn(PersonDataColumn inputColumn)
        {
            _personDataColumns.Add(inputColumn);
        }

        public IEnumerable<PersonDataColumn> GetColumns()
        {
            return _personDataColumns;
        }
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Empty_Query_when_No_TablePairs_Provided()
    {
        TestPersonDataColumnService personDataColumnService = new();
        Vacuumer vacuumer = new(personDataColumnService);
        var query = vacuumer.GenerateUpdateStatement();

        Assert.Empty(query);
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs()
    {
        TestPersonDataColumnService personDataColumnService = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "Null",
            deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);

        Vacuumer vacuumer = new(personDataColumnService);
        var query = vacuumer.GenerateUpdateStatement();


        var expected =
            "UPDATE Table SET Column = Null WHERE (Condition);";
        Assert.Contains(expected, query);
    }


    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_Purposes_One_TableColumnPair()
    {
        TestPersonDataColumnService personDataColumnService = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        DeleteCondition deleteCondition2 = new DeleteCondition("SecondCondition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition, deleteCondition2};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "Null",
            deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);

        Vacuumer vacuumer = new(personDataColumnService);
        var query = vacuumer.GenerateUpdateStatement();


        var expected =
            "UPDATE Table SET Column = Null WHERE (Condition) AND (SecondCondition);";
        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        TestPersonDataColumnService personDataColumnService = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "Null",
            deleteConditions);
        PersonDataColumn personDataColumn2 = new("SecondTable", "SecondColumn", "Null", deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);
        personDataColumnService.AddColumn(personDataColumn2);

        Vacuumer vacuumer = new(personDataColumnService);
        var query = vacuumer.GenerateUpdateStatement();


        var firstExpected =
            "UPDATE Table SET Column = Null WHERE (Condition);";
        var secondExpected = "UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);";
        Assert.Contains(firstExpected, query);
        Assert.Contains(secondExpected, query);
    }


    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs_With_Default_UpdateValue_Defined()
    {
        TestPersonDataColumnService personDataColumnService = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "UpdateValue",
            deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);

        Vacuumer vacuumer = new(personDataColumnService);
        var query = vacuumer.GenerateUpdateStatement();


        var expected =
            "UPDATE Table SET Column = UpdateValue WHERE (Condition);";
        Assert.Contains(expected, query);
    }
}