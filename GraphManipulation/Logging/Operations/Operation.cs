using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Logging.Operations;

public abstract class Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Operation(SystemOperation.Operation operationName, TKey key, TValue? value = null)
    {
        OperationName = operationName;
        Key = key;
        Value = value;
    }

    public SystemOperation.Operation OperationName { get; } // The name of the operation
    public TKey Key { get; } // The identifier of the subject
    public TValue? Value { get; protected set; }

    public override string ToString()
    {
        return FeedbackEmitterMessage.ResultMessage(Key, OperationName, null, Value);
    }
}