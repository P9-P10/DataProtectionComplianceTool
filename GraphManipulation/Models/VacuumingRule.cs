namespace GraphManipulation.Models;

public class VacuumingRule
{
    public int Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
    public string Rule { get; private set; }
    public IEnumerable<Purpose> Purposes { get; set; }
}