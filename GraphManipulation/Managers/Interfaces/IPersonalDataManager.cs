using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IPersonalDataManager : 
    IGetter<IPersonalDataColumn, TableColumnPair>, 
    IDeleter<TableColumnPair>, 
    IDescriptionUpdater<TableColumnPair>,
    IHasPurposes<TableColumnPair, string>
{
    public void AddPersonalData(TableColumnPair tableColumnPair, string joinCondition, string description);

    public void SetOriginOf(TableColumnPair tableColumnPair, int individualsId, string originName);
    public IOrigin? GetOriginOf(TableColumnPair tableColumnPair, int individualsId);
}