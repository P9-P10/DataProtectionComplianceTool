using System.ComponentModel.DataAnnotations.Schema;

namespace GraphManipulation.DataAccess.Entities;

[Table("personal_data_processing")]
public class ProcessingMetadata
{
    public int Id { get; set; }
    public string Process { get; set; }
    
    public int MetadataId { get; set; }
    public GdprMetadata Metadata { get; set; }
}