using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using GraphManipulation.DataAccess;
using GraphManipulation.Models;

namespace Test.Vacuuming.TestClasses;

public class TestVacuumingPolicyMapper : IMapper<VacuumingPolicy>
{
    private readonly List<VacuumingPolicy> _vacuumingPolicies = new();

    public VacuumingPolicy Insert(VacuumingPolicy value)
    {
        _vacuumingPolicies.Add(value);
        value.Id = _vacuumingPolicies.Count - 1;
        return value;
    }

    public IEnumerable<VacuumingPolicy> Find(Func<VacuumingPolicy, bool> condition)
    {
        return _vacuumingPolicies.Where(condition).ToList();
    }

    public VacuumingPolicy FindSingle(Func<VacuumingPolicy, bool> condition)
    {
        return _vacuumingPolicies.First(condition);
    }

    public VacuumingPolicy Update(VacuumingPolicy value)
    {
        if (value.Id == null)
            throw new EntityException("Value without identifier provided");
        _vacuumingPolicies[(int) value.Id] = value;
        return value;
    }

    public void Delete(VacuumingPolicy value)
    {
        _vacuumingPolicies.Remove(value);
    }

    public List<VacuumingPolicy> FetchVacuumingPolicies()
    {
        return _vacuumingPolicies;
    }
}