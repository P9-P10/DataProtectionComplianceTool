namespace GraphManipulation.Models;

public class Purpose : DomainEntity
{
    public string? Description { get; set; }
    public string Name { get; set; }
    public bool LegallyRequired { get; set; }
    public IEnumerable<PersonalDataColumn> Columns { get; set; }
    public IEnumerable<VacuumingRule> Rules { get; set; }
    
    public DataAccess.Entities.DeleteCondition DeleteCondition { get; set; }
}