using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IOriginManager : 
    IGetter<Origin, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public Origin AddOrigin(string name, string description);
}