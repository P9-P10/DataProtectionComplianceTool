using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IPurpose : INamedEntity, IDescribedEntity, IListable
{
    public bool GetLegallyRequired();
    public IEnumerable<string> GetDeleteCondition();
}