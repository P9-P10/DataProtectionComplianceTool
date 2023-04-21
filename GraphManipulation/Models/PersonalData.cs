namespace GraphManipulation.Models;

public class PersonalData : DomainEntity
{
    public Individual Individual { get; set; }
    public PersonalDataColumn Column { get; set; }
    public Origin Origin { get; set; }
}