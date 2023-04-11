using System.Collections.Generic;
using GraphManipulation.Vacuuming;

namespace Test.Vacuuming.TestClasses;

public class TestVacuumerStore : IVacuumerStore
{
    public int StoreVacuumingRule(string purpose, string interval)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<VacuumingRule> FetchVacuumingRules()
    {
        throw new System.NotImplementedException();
    }

    public VacuumingRule FetchVacuumingRule(int id)
    {
        throw new System.NotImplementedException();
    }
}