using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IVacuumingRule : INamedEntity, IDescribedEntity, IListable
{
    public string GetInterval();
    public IEnumerable<IPurpose> GetPurposes();
}