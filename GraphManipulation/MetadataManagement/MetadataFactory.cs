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
}