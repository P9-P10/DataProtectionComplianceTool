using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IProcessingManager : 
    IGetter<Processing, TableColumnPair>,
    IDescriptionUpdater<TableColumnPair>,
    IDeleter<TableColumnPair>
{
    public void AddProcessing(TableColumnPair tableColumnPair, string purposeName, string description);
    public void UpdatePurpose(TableColumnPair tableColumnPair, string purposeName);
}