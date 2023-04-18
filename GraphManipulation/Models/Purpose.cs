namespace GraphManipulation.Models;

public class Purpose
{

    public int? Id { get;  set; }
    public string Description { get;  set; }
    public string Name { get;  set; }
    public IEnumerable<Column>? Columns { get; set; }
    public IEnumerable<VacuumingRule>? Rules { get; set; }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as Purpose);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Id,Description);
    }

    bool Equals(Purpose? other)
    {
        return other.Description == Description && other.Name == Name && other.Id == Id;
    }
}