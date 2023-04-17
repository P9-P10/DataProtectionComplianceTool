namespace GraphManipulation.Models;

public class PersonDataColumn
{
    public int Identifier { get; }
    public string Description { get; }
    public string Name { get; }
    public IEnumerable<Purpose> Purposes { get; set; }
    public Origin Origin { get; set; }
}