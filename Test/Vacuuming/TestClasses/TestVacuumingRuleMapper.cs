﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess;
using GraphManipulation.Models;
using GraphManipulation.SchemaEvolution.Models.Entity;

namespace Test.Vacuuming.TestClasses;

public class TestVacuumingRuleMapper : IMapper<VacuumingRule>
{
    private readonly List<VacuumingRule> _vacuumingRules = new();

    public VacuumingRule Insert(VacuumingRule value)
    {
        _vacuumingRules.Add(value);
        value.Id = _vacuumingRules.Count - 1;
        return value;
    }

    public IEnumerable<VacuumingRule> Find(Func<VacuumingRule, bool> condition)
    {
        return _vacuumingRules.Where(condition).ToList();
    }

    public VacuumingRule FindSingle(Func<VacuumingRule, bool> condition)
    {
        return _vacuumingRules.First(condition);
    }

    public VacuumingRule Update(VacuumingRule value)
    {
        if (value.Id == null)
            throw new EntityException("Value without identifier provided");
        _vacuumingRules[(int) value.Id] = value;
        return value;
    }

    public void Delete(VacuumingRule value)
    {
        _vacuumingRules.Remove(value);
    }

    public List<VacuumingRule> FetchVacuumingRules()
    {
        return _vacuumingRules;
    }
}