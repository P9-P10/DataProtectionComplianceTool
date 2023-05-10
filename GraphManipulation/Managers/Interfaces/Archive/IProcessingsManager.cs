using GraphManipulation.Managers.Archive;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces.Archive;

public interface IProcessingsManager : 
    IGetter<IProcessing, string>,
    IDescriptionUpdater<string>,
    IDeleter<string>,
    INameUpdater
{
    public void AddProcessing(string name, TableColumnPair tableColumnPair, string purposeName, string description);
}