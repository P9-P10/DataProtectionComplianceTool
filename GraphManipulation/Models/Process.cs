namespace GraphManipulation.Models;

public class Process
{
    public int? Id { get; set; }
    public string Name { get; set; }
    public Purpose Purpose { get; set; }
    public Column Column { get; set; }
}