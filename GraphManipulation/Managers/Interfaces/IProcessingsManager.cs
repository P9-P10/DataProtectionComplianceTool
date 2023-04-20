using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IProcessingsManager : 
    IGetter<IProcessing, TableColumnPair>,
    IDescriptionUpdater<TableColumnPair>,
    IDeleter<TableColumnPair>
{
    public void AddProcessing(TableColumnPair tableColumnPair, string purposeName, string description);
    public void UpdatePurpose(TableColumnPair tableColumnPair, string purposeName);
}