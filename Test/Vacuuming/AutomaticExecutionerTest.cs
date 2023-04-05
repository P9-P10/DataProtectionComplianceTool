using System;
using System.Collections.Generic;
using System.Globalization;
using GraphManipulation.Vacuuming;
using GraphManipulation.Vacuuming.Components;
using Xunit;

namespace Test.Vacuuming;

public class AutomaticExecutionerTest
{
    private class TestExecutionFetcher : IExecutionsFetcher
    {
        public List<Execution> FetchExecutions()
        {
            return new List<Execution>();
        }

        public void StoreExecutions(List<Execution> executions)
        {
            foreach (var execution in executions)
            {
                StoreExecution(execution);
            }
        }

        public void StoreExecution(Execution execution)
        {
        }
    }
    
    private class TestExecutionParser : IExecutionParser
    {
        public Execution ParseExecutionPolicy(string executionInterval, string policy)
        {
            return new Execution(policy, executionInterval, DateTime.Now.Date);
        }

        public string ParseExecution(Execution execution)
        {
            return "";
        }
    }

    [Fact]
    public void TestAddAutomaticExecution()
    {
        IExecutionsFetcher executionFetcher = new TestExecutionFetcher();
        IExecutionParser executionParser = new TestExecutionParser();
        IAutomaticExecutioner automaticExecutioner = new AutomaticExecutioner(executionFetcher,executionParser);

        automaticExecutioner.AddNewAutomaticExecution("Policy", "2h 3d 4m 5y",
            DateTime.ParseExact("2023-05-04 12:00", "yyyy-M-d h:m", CultureInfo.InvariantCulture));

        Execution execution = executionParser.ParseExecutionPolicy("2h 3d 4m 5y", "Policy");

        Assert.Contains(execution, automaticExecutioner.GetAutomaticExecutions());
    }

    /*
    [Fact]
    public void TestGetNextExecutionTime()
    {
        IExecutionsFetcher executionFetcher = new TestExecutionFetcher();
        IAutomaticExecutioner automaticExecutioner = new AutomaticExecutioner(executionFetcher);
        automaticExecutioner.AddNewAutomaticExecution("Policy", "2h 3d 4m 5y",
            DateTime.ParseExact("2023-05-04 12:00", "yyyy-M-d h:m", CultureInfo.InvariantCulture));
        automaticExecutioner.GetNextExecution("0 0/0 0/2 4/3 5/4 ? 2023/5");
    }*/
}