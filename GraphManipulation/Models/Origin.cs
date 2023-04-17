
namespace GraphManipulation.Models;

public class Origin
{
    public int Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; }
    public IEnumerable<PersonDataColumn> Columns { get; set; }
}