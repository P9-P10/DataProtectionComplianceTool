namespace GraphManipulation.Models;

public class VacuumingRule
{
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
    public string Name { get;  set; }
    public string Interval { get;  set; }
    public IEnumerable<Purpose>? Purposes { get; set; }

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