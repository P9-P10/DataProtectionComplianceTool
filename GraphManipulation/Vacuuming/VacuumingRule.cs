namespace GraphManipulation.Vacuuming;

public class VacuumingRule
{
    public string Interval { get; set; }

    public string Purpose { get; set; }

    public string RuleName { get; set; }

    public VacuumingRule(string ruleName, string purpose, string interval)
    {
        Purpose = purpose;
        Interval = interval;
        RuleName = ruleName;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VacuumingRule);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Interval, Purpose, RuleName);
    }

    bool Equals(VacuumingRule? other)
    {
        return other.Purpose == Purpose && other.Interval == Interval && other.RuleName == RuleName;
    }
}