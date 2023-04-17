namespace GraphManipulation.Models;

public class Purpose
{
    public Purpose(int identifier, string description, string name, IEnumerable<PersonDataColumn>? columns = null,
        IEnumerable<VacuumingRule>? rules = null)
    {
        Identifier = identifier;
        Description = description;
        Name = name;
        columns ??= new List<PersonDataColumn>();
        Columns = columns;
        rules ??= new List<VacuumingRule>();
        Rules = rules;
    }
    
    public Purpose(string name, string description, IEnumerable<PersonDataColumn>? columns = null,
        IEnumerable<VacuumingRule>? rules = null)
    {
        Description = description;
        Name = name;
        columns ??= new List<PersonDataColumn>();
        Columns = columns;
        rules ??= new List<VacuumingRule>();
        Rules = rules;
    }

    public int? Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
    public IEnumerable<PersonDataColumn>? Columns { get; set; }
    public IEnumerable<VacuumingRule>? Rules { get; set; }
}