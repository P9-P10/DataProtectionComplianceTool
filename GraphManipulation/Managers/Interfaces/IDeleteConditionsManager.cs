using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces;

public interface IDeleteConditionsManager : 
    IGetter<IDeleteCondition, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public void Add(string name, string description, string condition);
    public void UpdateCondition(string name, string condition);
}