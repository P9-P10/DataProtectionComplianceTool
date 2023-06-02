using System.Text.RegularExpressions;

namespace GraphManipulation.Models;

public class VacuumingPolicy : Entity<string>
{
    private string? _duration;

    public VacuumingPolicy(int id, string description, string name, string duration,
        IEnumerable<Purpose>? purposes = null)
    {
        Id = id;
        Description = description;
        Key = name;
        Duration = duration;
        purposes ??= new List<Purpose>();
        Purposes = purposes;
    }

    public VacuumingPolicy(string description, string name, string duration,
        IEnumerable<Purpose>? purposes = null)
    {
        Description = description;
        Key = name;
        Duration = duration;
        purposes ??= new List<Purpose>();
        Purposes = purposes;
    }

    public VacuumingPolicy(string name, string description, string duration)
    {
        Description = description;
        Key = name;
        Duration = duration;
        Purposes = new List<Purpose>();
    }

    public VacuumingPolicy(string name, string duration, List<Purpose> purposes)
    {
        Key = name;
        Duration = duration;
        Purposes = purposes;
    }

    public VacuumingPolicy(string name, string duration)
    {
        Key = name;
        Duration = duration;
    }

    public VacuumingPolicy()
    {
        // This constructor is needed.
    }

    public string? Duration
    {
        get => _duration;
        set
        {
            if (IsValidDuration(value))
            {
                _duration = value;
            }
            else
            {
                throw new DurationParseException();
            }
        }
    }

    public DateTime? LastExecution { get; set; }

    public virtual IEnumerable<Purpose>? Purposes { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(Duration),
            NullToString(LastExecution),
            ListNullOrEmptyToString(Purposes));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Duration", "Last Execution", "Purposes");
    }

    public static bool IsValidDuration(string? duration)
    {
        return duration is not null && Regex.Match(duration, @"(\d+(y|d|m|M|D|w) {0,1})+").Success;
    }

    public bool ShouldExecute()
    {
        return GetNextExecutionDate() <= DateTime.Now;
    }

    private DateTime GetNextExecutionDate()
    {
        ParsedDuration duration = DurationParser();
        if (LastExecution == null)
        {
            return DateTime.Now;
        }

        return LastExecution.Value.AddYears(duration.years).AddMonths(duration.months).AddDays(duration.days)
            .AddHours(duration.hours).AddMinutes(duration.minutes);
    }

    private ParsedDuration DurationParser()
    {
        ParsedDuration duration = new();
        string[] components = Duration.Split(" ");
        foreach (var component in components)
        {
            if (component.Contains('M'))
            {
                duration.minutes = GetTimeFromComponent(component);
            }

            if (component.Contains('h'))
            {
                duration.hours = GetTimeFromComponent(component);
            }

            if (component.Contains('d'))
            {
                duration.days = GetTimeFromComponent(component);
            }

            if (component.Contains('m'))
            {
                duration.months = GetTimeFromComponent(component);
            }

            if (component.Contains('y'))
            {
                duration.years = GetTimeFromComponent(component);
            }
        }

        return duration;
    }

    private int GetTimeFromComponent(string input)
    {
        return int.Parse(Regex.Match(input, @"\d+").Value);
    }

    private struct ParsedDuration
    {
        public int minutes = 0;
        public int hours = 0;
        public int days = 0;
        public int months = 0;
        public int years = 0;

        public ParsedDuration()
        {
        }
    }

    public class DurationParseException : Exception
    {
    }
}