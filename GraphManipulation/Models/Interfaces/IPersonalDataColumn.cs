using GraphManipulation.Managers;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IPersonalDataColumn : IDescribedEntity, IListable
{
    public TableColumnPair GetTableColumnPair();
    public IEnumerable<IPurpose> GetPurposes();
    public string GetDefaultValue();
}