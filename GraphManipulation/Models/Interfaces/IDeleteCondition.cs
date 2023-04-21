using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IDeleteCondition : INamedEntity, IDescribedEntity, IListable
{
    public string GetCondition();
}