using System;
using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;

namespace Test.Vacuuming.TestClasses;

public class TestPersonalDataColumnMapper : IMapper<PersonalDataColumn>
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

    public PersonalDataColumn Insert(PersonalDataColumn value)
    {
        _personDataColumns.Add(value);
        value.Id = _personDataColumns.Count - 1;
        return value;
    }

    public IEnumerable<PersonalDataColumn> Find(Func<PersonalDataColumn, bool> condition)
    {
        return _personDataColumns.Where(condition);
    }

    public PersonalDataColumn? FindSingle(Func<PersonalDataColumn, bool> condition)
    {
        return _personDataColumns.Where(condition).First();
    }

    public PersonalDataColumn Update(PersonalDataColumn value)
    {
        int index = _personDataColumns.FindIndex(x => x.Id == value.Id);
        _personDataColumns[index] = value;
        return _personDataColumns[index];
    }

    public void Delete(PersonalDataColumn value)
    {
        _personDataColumns.Remove(value);
    }
}