namespace GraphManipulation.Models;

public class Purpose
{
    public int Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
    public IEnumerable<PersonDataColumn> Columns { get; set; }
    public IEnumerable<VacuumingRule> Rules { get; set; }
}