using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models;

public class Processing : DomainEntity, IProcessing
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IPurpose Purpose { get; set; }
    public IPersonalDataColumn PersonalDataColumn { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Purpose.ToListing(), PersonalDataColumn.ToListing());
    }
}