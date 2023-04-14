using GraphManipulation.DataAccess.Entities;

namespace GraphManipulation.MetadataManagement;

public static class MetadataFactory
{
    public static GDPRMetadata CreateMetadata()
    {
        return new GDPRMetadata();
    }

    public static GDPRMetadata For(this GDPRMetadata metadata, ColumnMetadata column)
    {
        metadata.TargetTable = column.TargetTable;
        metadata.TargetColumn = column.TargetColumn;
        return metadata;
    }

    public static GDPRMetadata WithPurpose(this GDPRMetadata metadata, string purpose)
    {
        metadata.Purpose = purpose;
        return metadata;
    }

    public static GDPRMetadata From(this GDPRMetadata metadata, string origin)
    {
        metadata.Origin = origin;
        return metadata;
    }

    public static GDPRMetadata IsLegallyRequired(this GDPRMetadata metadata, bool? legallyRequired)
    {
        metadata.LegallyRequired = legallyRequired;
        return metadata;
    }
}