using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class PersonalData : DomainEntity
{
    public virtual Individual Individual { get; set; }
    public virtual PersonalDataColumn PersonalDataColumn { get; set; }
    public virtual Origin Origin { get; set; }
}