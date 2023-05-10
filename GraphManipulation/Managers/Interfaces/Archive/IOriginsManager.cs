using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces.Archive;

public interface IOriginsManager : 
    IGetter<IOrigin, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public void Add(string name, string description);
}