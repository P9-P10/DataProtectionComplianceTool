namespace GraphManipulation.Vacuuming;

public class VacuumingRule
{
    public string Interval { get; set; }

    public string Purpose { get; set; }

    public string Rule { get; set; }

    public VacuumingRule(string rule, string purpose, string interval)
    {
        Purpose = purpose;
        Interval = interval;
        Rule = rule;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VacuumingRule);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Interval, Purpose, Rule);
    }

    bool Equals(VacuumingRule? other)
    {
        return other.Purpose == Purpose && other.Interval == Interval && other.Rule == Rule;
    }
}