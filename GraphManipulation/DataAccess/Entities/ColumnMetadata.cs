using System.ComponentModel.DataAnnotations.Schema;

namespace GraphManipulation.DataAccess.Entities;

[Table("metadata_columns")]
public class ColumnMetadata
{
    public int Id { get; set; }
    [Column("target_table")]
    public string TargetTable { get; set; }
    [Column("target_column")]
    public string TargetColumn { get; set; }
    [Column("default_value")]
    public string DefaultValue { get; set; }
    
    public ICollection<GdprMetadataEntity> Metadata { get; }
}