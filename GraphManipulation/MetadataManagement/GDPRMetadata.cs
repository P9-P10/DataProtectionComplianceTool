namespace GraphManipulation.Models.Metadata;

public class GDPRMetadata
{
    // These properties are named to be consistent with the corresponding columns in the database
    public string purpose { get; set; }
    public string ttl { get; set; }
    public string target_table { get; set; }
    public string target_column { get; set; }
    public string origin { get; set; }
    public string start_time { get; set; }
    public bool? legally_required { get; set; }

    // Parameterless constructor.
    // According to https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-initialize-objects-by-using-an-object-initializer 
    // This is needed in order to allow for the use of object initializers
    public GDPRMetadata() { }

    public GDPRMetadata(string targetTable, string targetColumn)
    {
        target_table = targetTable;
        target_column = targetColumn;
    }

    protected bool Equals(GDPRMetadata other)
    {
        return purpose == other.purpose && ttl == other.ttl && target_table == other.target_table && target_column == other.target_column && origin == other.origin && start_time == other.start_time && legally_required == other.legally_required;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((GDPRMetadata)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(purpose, ttl, target_table, target_column, origin, start_time, legally_required);
    }
}