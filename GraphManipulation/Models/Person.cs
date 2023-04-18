using System.Collections;

namespace GraphManipulation.Models;

public class Person : DomainEntity
{
    public IEnumerable<Column> Columns { get; set; }
    // TODO: Link person to metadata that is relevant ot them
}