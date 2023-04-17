namespace GraphManipulation.Models;

public class PersonDataColumn
{
    public PersonDataColumn(int identifier, string description, string name, IEnumerable<Purpose> purposes, Origin origin)
    {
        Identifier = identifier;
        Description = description;
        Name = name;
        Purposes = purposes;
        Origin = origin;
    }

    public int Identifier { get; }
    public string Description { get; }
    public string Name { get; }
    public IEnumerable<Purpose> Purposes { get; set; }
    public Origin Origin { get; set; }
}