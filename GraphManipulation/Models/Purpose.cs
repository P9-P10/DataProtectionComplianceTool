namespace GraphManipulation.Models;

public class Purpose
{
    public Purpose(int identifier, string description, string name, IEnumerable<Column>? columns = null,
        IEnumerable<VacuumingRule>? rules = null)
    {
        Identifier = identifier;
        Description = description;
        Name = name;
        columns ??= new List<Column>();
        Columns = columns;
        rules ??= new List<VacuumingRule>();
        Rules = rules;
    }
    
    public Purpose(string name, string description, IEnumerable<Column>? columns = null,
        IEnumerable<VacuumingRule>? rules = null)
    {
        Description = description;
        Name = name;
        columns ??= new List<Column>();
        Columns = columns;
        rules ??= new List<VacuumingRule>();
        Rules = rules;
    }

    public int? Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
    public IEnumerable<Column>? Columns { get; set; }
    public IEnumerable<VacuumingRule>? Rules { get; set; }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as Purpose);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Identifier,Description);
    }

    bool Equals(Purpose? other)
    {
        return other.Description == Description && other.Name == Name && other.Identifier == Identifier;
    }
}