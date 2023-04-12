using GraphManipulation.MetadataManagement.AttributeMapping;

namespace GraphManipulation.MetadataManagement;

public class GDPRMetadata
{
    // Parameterless constructor.
    // According to https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/how-to-initialize-objects-by-using-an-object-initializer 
    // This is needed in order to allow for the use of object initializers
    public GDPRMetadata()
    {
    }

    public GDPRMetadata(string targetTable, string targetColumn)
    {
        TargetTable = targetTable;
        TargetColumn = targetColumn;
    }

    public int Id { get; set; }
    [Column("purpose")] public string? Purpose { get; init; }

    [Column("target_table")] public string? TargetTable { get; set; }

    [Column("target_column")] public string? TargetColumn { get; set; }

    [Column("origin")] public string? Origin { get; init; }

    [Column("legally_required")] public bool? LegallyRequired { get; init; }

    protected bool Equals(GDPRMetadata other)
    {
        return Purpose == other.Purpose && TargetTable == other.TargetTable &&
               TargetColumn == other.TargetColumn && Origin == other.Origin &&
               LegallyRequired == other.LegallyRequired;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((GDPRMetadata)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Purpose, TargetTable, TargetColumn, Origin, LegallyRequired);
    }
}