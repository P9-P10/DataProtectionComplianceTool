namespace GraphManipulation.Models;

public class VacuumingRule
{
    public int? Id { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public string Rule { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }
}