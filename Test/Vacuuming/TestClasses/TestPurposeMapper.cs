using System;
using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;

namespace Test.Vacuuming.TestClasses;

public class TestPurposeMapper : IMapper<Purpose>
{
    private readonly List<Purpose> _purposes = new();


    public Purpose Insert(Purpose value)
    {
        _purposes.Add(value);
        value.Id = _purposes.Count - 1;
        return value;
    }

    public IEnumerable<Purpose> Find(Func<Purpose, bool> condition)
    {
        return _purposes.Where(condition);
    }

    public Purpose? FindSingle(Func<Purpose, bool> condition)
    {
        return _purposes.Where(condition).First();
    }

    public Purpose Update(Purpose value)
    {
        int index = _purposes.FindIndex(x => x.Id == value.Id);
        _purposes[index] = value;
        return _purposes[index];
    }

    public void Delete(Purpose value)
    {
        _purposes.Remove(value);
    }
}