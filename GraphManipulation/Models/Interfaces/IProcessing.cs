using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IProcessing : INamedEntity, IDescribedEntity, IListable
{
    public IPurpose Purpose { get; set; }
    public IPersonalDataColumn PersonalDataColumn { get; set; }
}