namespace GraphManipulation.Models;

public class PersonalData : DomainEntity
{
    public Individual Individual { get; set; }
    public PersonalDataColumn PersonalDataColumn { get; set; }
    public Origin Origin { get; set; }
}