using System.Text.RegularExpressions;

namespace GraphManipulation.Models;

public class VacuumingRule : Entity<string>
{
    private string? _interval;

    public VacuumingRule(int id, string description, string name, string interval,
        IEnumerable<Purpose>? purposes = null)
    {
        Id = id;
        Description = description;
        Key = name;
        Interval = interval;
        purposes ??= new List<Purpose>();
        Purposes = purposes;
    }

    public VacuumingRule(string description, string name, string interval,
        IEnumerable<Purpose>? purposes = null)
    {
        Description = description;
        Key = name;
        Interval = interval;
        purposes ??= new List<Purpose>();
        Purposes = purposes;
    }

    public VacuumingRule(string name, string description, string interval)
    {
        Description = description;
        Key = name;
        Interval = interval;
        Purposes = new List<Purpose>();
    }

    public VacuumingRule(string name, string interval, List<Purpose> purposes)
    {
        Key = name;
        Interval = interval;
        Purposes = purposes;
    }

    public VacuumingRule(string name, string interval)
    {
        Key = name;
        Interval = interval;
    }

    public VacuumingRule()
    {
        // This constructor is needed.
    }

    public string? Interval
    {
        get => _interval;
        set
        {
            if (IsValidInterval(value))
            {
                _interval = value;
            }
            else
            {
                throw new IntervalParseException();
            }
        }
    }

    public DateTime? LastExecution { get; set; }

    public virtual IEnumerable<Purpose>? Purposes { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(Interval),
            NullToString(LastExecution),
            ListNullOrEmptyToString(Purposes));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Interval", "Last Execution", "Purposes");
    }

    public static bool IsValidInterval(string? interval)
    {
        return interval is not null && Regex.Match(interval, @"(\d+(y|d|m|M|D|w) {0,1})+").Success;
    }

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

    public class IntervalParseException : Exception
    {
    }
}