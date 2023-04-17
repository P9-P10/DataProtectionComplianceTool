namespace GraphManipulation.Models;

public class Purpose
{
    public int? Id { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public bool LegallyRequired { get; set; }
    public IEnumerable<Column> Columns { get; set; }
    public IEnumerable<VacuumingRule> Rules { get; set; }
}