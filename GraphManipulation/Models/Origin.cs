
namespace GraphManipulation.Models;

public class Origin
{
    public int Id { get; set; }
    public string Name { get; }
    public IEnumerable<Column> Columns { get; set; }
}