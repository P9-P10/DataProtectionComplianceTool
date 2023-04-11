using System.Collections.Generic;
using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;

namespace Test.Vacuuming.TestClasses;

public class TestPersonDataColumnService : IPersonDataColumnService
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