using System.Text.RegularExpressions;

namespace GraphManipulation.Models;

public class VacuumingRule
{
    private struct ParsedInterval
    {
        public int minutes = 0;
        public int hours = 0;
        public int days = 0;
        public int months = 0;
        public int years = 0;

        public ParsedInterval()
        {
        }
    }

    public VacuumingRule(int id, string description, string name, string interval,
        IEnumerable<Purpose>? purposes = null)
    {
        Id = id;
        Description = description;
        Name = name;
        Interval = interval;
        purposes ??= new List<Purpose>();
        Purposes = purposes;
    }

    public VacuumingRule(string description, string name, string interval,
        IEnumerable<Purpose>? purposes = null)
    {
        Description = description;
        Name = name;
        Interval = interval;
        purposes ??= new List<Purpose>();
        Purposes = purposes;
    }

    public VacuumingRule(string name, string description, string interval)
    {
        Description = description;
        Name = name;
        Interval = interval;
        Purposes = new List<Purpose>();
    }

    public VacuumingRule()
    {
    }

    public int? Id { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public string Interval { get; set; }

    public DateTime? LastExecution { get; set; }
    public IEnumerable<Purpose>? Purposes { get; set; }

    public bool ShouldExecute()
    {
        return GetNextExecutionDate() <= DateTime.Now;
    }

    private DateTime GetNextExecutionDate()
    {
        ParsedInterval interval = IntervalParser();
        if (LastExecution == null)
        {
            return DateTime.Now;
        }

        return LastExecution.Value.AddYears(interval.years).AddMonths(interval.months).AddDays(interval.days)
            .AddHours(interval.hours).AddMinutes(interval.minutes);
    }

    private ParsedInterval IntervalParser()
    {
        ParsedInterval interval = new();
        string[] components = Interval.Split(" ");
        foreach (var component in components)
        {
            if (component.Contains('M'))
            {
                interval.minutes = GetTimeFromComponent(component);
            }

            if (component.Contains('h'))
            {
                interval.hours = GetTimeFromComponent(component);
            }

            if (component.Contains('d'))
            {
                interval.days = GetTimeFromComponent(component);
            }

            if (component.Contains('m'))
            {
                interval.months = GetTimeFromComponent(component);
            }

            if (component.Contains('y'))
            {
                interval.years = GetTimeFromComponent(component);
            }
        }

        return interval;
    }

    private int GetTimeFromComponent(string input)
    {
        return int.Parse(Regex.Match(input, @"\d+").Value);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VacuumingRule);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Interval, Name, Id);
    }

    bool Equals(VacuumingRule? other)
    {
        return other.Interval == Interval && other.Name == Name && other.Id == Id;
    }
}