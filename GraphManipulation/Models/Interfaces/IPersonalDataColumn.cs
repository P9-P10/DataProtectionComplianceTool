using GraphManipulation.Managers;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IPersonalDataColumn : IDescribedEntity, IListable
{
    public TableColumnPair TableColumnPair { get; set; }
    public IEnumerable<IPurpose> Purposes { get; set; }
}