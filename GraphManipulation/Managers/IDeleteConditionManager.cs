using GraphManipulation.DataAccess.Entities;

namespace GraphManipulation.Managers;

public interface IDeleteConditionManager
{
    public DeleteCondition AddDeleteCondition(string name, string description, string condition);
    public DeleteCondition UpdateDeleteConditionName(string name, string newName);
    public DeleteCondition UpdateDeleteConditionDescription(string name, string description);
    public DeleteCondition UpdateDeleteConditionCondition(string name, string condition);
    public void DeleteDeleteCondition(string name);
    public IEnumerable<DeleteCondition> GetAllDeleteConditions();
    public DeleteCondition? GetDeleteCondition(string name);
}