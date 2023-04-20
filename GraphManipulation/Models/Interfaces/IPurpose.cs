using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IPurpose : INamedEntity, IDescribedEntity, IListable
{
    public bool LegallyRequired { get; set; }
    public IDeleteCondition? DeleteCondition { get; set; }
}