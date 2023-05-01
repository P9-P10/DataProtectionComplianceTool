using System.Collections.Generic;
using GraphManipulation.Models;
using GraphManipulation.Services;

namespace Test.Vacuuming.TestClasses;

public class TestPersonDataColumnService : IPersonDataColumnService
{
    private readonly List<PersonalDataColumn> _personDataColumns = new();

    public void AddColumn(PersonalDataColumn inputColumn)
    {
        _personDataColumns.Add(inputColumn);
    }

    public IEnumerable<PersonalDataColumn> GetColumns()
    {
        return _personDataColumns;
    }
}