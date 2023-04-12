using System.ComponentModel.DataAnnotations.Schema;

namespace GraphManipulation.DataAccess.Entities;

[Table("gdpr_metadata")]
public class GdprMetadata
{
    public int Id { get; set; }
    public string? Purpose { get; set; }
    public string? Origin { get; set; }
    [Column("legally_required")]
    public bool? LegallyRequired { get; set; }
    
    [Column("target_column")]
    public int TargetColumn { get; set; }
    public ColumnMetadata Column { get; set; }
    public ICollection<ConditionMetadata> Conditions { get; }
    public ICollection<ProcessingMetadata> Processes { get; }
}