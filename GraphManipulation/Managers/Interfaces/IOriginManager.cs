using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IOriginManager : 
    IGetter<IOrigin, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public void AddOrigin(string name, string description);
}