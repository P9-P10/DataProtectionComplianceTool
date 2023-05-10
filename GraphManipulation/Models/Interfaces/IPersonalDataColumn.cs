using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Interfaces;

public interface IPersonalDataColumn : IDescribedEntity, IListable
{
    public TableColumnPair GetTableColumnPair();
    public IEnumerable<IPurpose> GetPurposes();
    public string GetJoinCondition();
    public string GetDefaultValue();
}