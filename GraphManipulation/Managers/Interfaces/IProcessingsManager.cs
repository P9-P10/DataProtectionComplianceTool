using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IProcessingsManager : 
    IGetter<IProcessing, string>,
    IDescriptionUpdater<string>,
    IDeleter<string>,
    INameUpdater
{
    public void AddProcessing(string name, TableColumnPair tableColumnPair, string purposeName, string description);
    public void UpdatePurpose(string name, TableColumnPair tableColumnPair, string purposeName);
}