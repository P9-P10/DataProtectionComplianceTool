using System.ComponentModel.DataAnnotations.Schema;

namespace GraphManipulation.DataAccess.Entities;

[Table("delete_conditions")]
public class ConditionMetadata
{
    public int Id { get; set; }
    public string Condition { get; set; }
    
    public int MetadataId { get; set; }
    public GdprMetadata Metadata { get; set; }
}