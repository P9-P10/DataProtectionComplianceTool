using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IOrigin : INamedEntity, IDescribedEntity, IListable
{
    public IEnumerable<IPersonalDataColumn> PersonalDataColumns { get; set; }
}