namespace GraphManipulation.Models;

public class Process : DomainEntity
{
    public string Name { get; set; }
    public Purpose Purpose { get; set; }
    public Column Column { get; set; }
}