using GraphManipulation.MetadataManagement.AttributeMapping;

namespace GraphManipulation.Models.Metadata;

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

    // These properties are named to be consistent with the corresponding columns in the database
    [Column("purpose")] public string Purpose { get; set; }

    [Column("ttl")] public string TTL { get; set; }

    [Column("target_table")] public string TargetTable { get; set; }

    [Column("target_column")] public string TargetColumn { get; set; }

    [Column("origin")] public string Origin { get; set; }

    [Column("start_time")] public string StartTime { get; set; }

    [Column("legally_required")] public bool? LegallyRequired { get; set; }

    // These properties are named to be consistent with the corresponding columns in the database
    public string purpose { get; set; }
    public string ttl { get; set; }
    public string target_table { get; set; }
    public string target_column { get; set; }
    public string origin { get; set; }
    public string start_time { get; set; }
    public bool? legally_required { get; set; }

    protected bool Equals(GDPRMetadata other)
    {
        return Purpose == other.Purpose && TTL == other.TTL && TargetTable == other.TargetTable &&
               TargetColumn == other.TargetColumn && Origin == other.Origin && StartTime == other.StartTime &&
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
        return HashCode.Combine(Purpose, TTL, TargetTable, TargetColumn, Origin, StartTime, LegallyRequired);
    }
}