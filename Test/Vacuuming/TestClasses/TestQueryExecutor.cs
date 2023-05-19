using System.Collections.Generic;
using GraphManipulation.Vacuuming;

namespace Test.Vacuuming.TestClasses;

public class TestQueryExecutor : IQueryExecutor
{
    public List<string> Query;

    public TestQueryExecutor()
    {
        Query = new List<string>();
    }

    public void Execute(string query)
    {
        if (!Query.Contains(query))
        {
            Query.Add(query);
        }
    }
}