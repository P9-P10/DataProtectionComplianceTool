using System.Text.RegularExpressions;
using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class AutomaticExecutioner : IAutomaticExecutioner
{
    private readonly IExecutionsFetcher _executionsFetcher;

    private readonly IExecutionParser _executionParser;

    private readonly List<Execution> _executions;

    public AutomaticExecutioner(IExecutionsFetcher executionsFetcher, IExecutionParser executionParser)
    {
        _executionsFetcher = executionsFetcher;
        _executionParser = executionParser;
        _executions = _executionsFetcher.FetchExecutions();
    }

    public void AddNewAutomaticExecution(string policy, string executeEvery, DateTime startDate)
    {
        Execution execution = _executionParser.ParseExecutionPolicy(executeEvery, policy);
        _executions.Add(execution);
        _executionsFetcher.StoreExecution(execution);
    }

    public void UpdateAutomaticExecution()
    {
    }

    public Execution GetNextExecution()
    {
        foreach (var VARIABLE in _executions)
        {
        }

        return new Execution("", "",DateTime.Now);
    }

    public List<Execution> GetAutomaticExecutions()
    {
        return _executions;
    }


    private string ConvertExecuteEveryToCron(string inputString, DateTime startDate)
    {
        int years = 0, months = 0, days = 0, hours = 0, minutes = 0;

        var components = inputString.Split(" ");

        foreach (var component in components)
        {
            var componentNumber = GetNumberFromComponentGetNumberFromComponent(component);
            switch (component)
            {
                case { } when component.Contains('y'):
                    years += componentNumber;
                    break;
                case { } when component.Contains('m'):
                    months += componentNumber;
                    break;
                case { } when component.Contains('d'):
                    days += componentNumber;
                    break;
                case { } when component.Contains('h'):
                    hours += componentNumber;
                    break;
                case { } when component.Contains('M'):
                    minutes += componentNumber;
                    break;
            }
        }


        return
            $"0 {startDate.Minute}/{minutes} {startDate.Hour}/{hours} {startDate.Day}/{days} {startDate.Month}/{months} ? {startDate.Year}/{years}";
    }

    private int GetNumberFromComponentGetNumberFromComponent(string inputString)
    {
        var match = Regex.Match(inputString, @"\d");
        return match.Success ? Convert.ToInt32(match.Value) : 0;
    }
}