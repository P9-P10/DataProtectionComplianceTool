using System.Collections.Generic;
using GraphManipulation.Vacuuming;

namespace Test.Vacuuming.TestClasses;

public class TestVacuumerStore : IVacuumerStore
{
    private List<VacuumingRule?> _rules;

    public TestVacuumerStore()
    {
        _rules = new List<VacuumingRule?>();
    }

    public int StoreVacuumingRule(VacuumingRule vacuumingRule)
    {
        if (_rules.Contains(vacuumingRule)) return _rules.IndexOf(vacuumingRule);
        _rules.Add(vacuumingRule);
        return _rules.Count;
    }

    public IEnumerable<VacuumingRule> FetchVacuumingRules()
    {
        return _rules;
    }

    public VacuumingRule? FetchVacuumingRule(int id)
    {
        return _rules[id-1];
    }

    public void DeleteVacuumingRule(int id)
    {
        _rules.RemoveAt(id-1);
    }

    public bool UpdateVacuumingRule(int id, VacuumingRule newRule)
    {
        if (_rules.Count >= id) return false;
        _rules[id] = newRule;
        return true;

    }
}