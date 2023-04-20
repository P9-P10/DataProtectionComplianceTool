using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IVacuumingRule : INamedEntity, IDescribedEntity, IListable
{
    public string Interval { get; set; }
}