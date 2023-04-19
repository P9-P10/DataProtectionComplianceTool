using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IPersonalDataManager : 
    IGetter<PersonalDataColumn, TableColumnPair>, 
    IDeleter<TableColumnPair>, 
    IDescriptionUpdater<TableColumnPair>
{
    public void AddPersonalData(TableColumnPair tableColumnPair, string joinCondition, string description);

    public void AddPurpose(TableColumnPair tableColumnPair, string purposeName);
    public void RemovePurpose(TableColumnPair tableColumnPair, string purposeName);

    public void SetOriginOf(TableColumnPair tableColumnPair, int individualsId, string originName);
    public Origin GetOriginOf(TableColumnPair tableColumnPair, int individualsId);
}