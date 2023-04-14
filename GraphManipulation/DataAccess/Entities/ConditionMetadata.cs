using System.ComponentModel.DataAnnotations.Schema;

namespace GraphManipulation.DataAccess.Entities;

[Table("delete_conditions")]
public class ConditionMetadata
{
    public int Id { get; set; }
    public string Condition { get; set; }
    
    [Column("metadata_id")]
    public int MetadataId { get; set; }
    public GdprMetadataEntity MetadataEntity { get; set; }
}