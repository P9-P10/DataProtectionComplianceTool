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
    
    // TODO: Man skal ikke kunne opdatere et purpose, så må du slette det nuværende processing og lave en ny
    public void UpdatePurpose(string name, TableColumnPair tableColumnPair, string purposeName);
}