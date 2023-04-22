using System.Collections;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;
using VDS.Common.Collections.Enumerations;

namespace GraphManipulation.Models;

public class Individual : DomainEntity, IIndividual
{
    
    public IEnumerable<PersonalData> PersonalData { get; set; }
    public string ToListing()
    {
        return Id == null ? "Unknown" : Id.ToString();
    }
}