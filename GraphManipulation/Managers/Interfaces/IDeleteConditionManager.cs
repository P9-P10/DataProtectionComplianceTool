using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IDeleteConditionManager : 
    IGetter<DeleteCondition, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public void Add(string name, string description, string condition);
    public void UpdateCondition(string name, string condition);
}