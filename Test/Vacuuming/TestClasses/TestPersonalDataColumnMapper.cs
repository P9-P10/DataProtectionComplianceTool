using System;
using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;

namespace Test.Vacuuming.TestClasses;

public class TestPersonalDataColumnMapper : IMapper<PersonalDataColumn>
{
    private readonly List<PersonalDataColumn> _personalDataColumns = new();

    public void AddColumn(PersonalDataColumn inputColumn)
    {
        _personalDataColumns.Add(inputColumn);
    }

    public IEnumerable<PersonalDataColumn> GetColumns()
    {
        return _personalDataColumns;
    }

    public PersonalDataColumn Insert(PersonalDataColumn value)
    {
        _personalDataColumns.Add(value);
        value.Id = _personalDataColumns.Count - 1;
        return value;
    }

    public IEnumerable<PersonalDataColumn> Find(Func<PersonalDataColumn, bool> condition)
    {
        return _personalDataColumns.Where(condition);
    }

    public PersonalDataColumn? FindSingle(Func<PersonalDataColumn, bool> condition)
    {
        return _personalDataColumns.Where(condition).First();
    }

    public PersonalDataColumn Update(PersonalDataColumn value)
    {
        int index = _personalDataColumns.FindIndex(x => x.Id == value.Id);
        _personalDataColumns[index] = value;
        return _personalDataColumns[index];
    }

    public void Delete(PersonalDataColumn value)
    {
        _personalDataColumns.Remove(value);
    }
}