using System.Collections;

namespace GraphManipulation.Models;

public class Person
{
    public int? Id { get; set; }
    public IEnumerable<Column> Columns { get; set; }
    // TODO: Link person to metadata that is relevant ot them
}