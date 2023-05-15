using GraphManipulation.Helpers;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Logging.Operations;

public abstract class Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public OperationName Name { get; } // The name of the operation
    public TKey Key { get; } // The identifier of the subject
    public TValue? Value { get; protected set; }

    public Operation(OperationName operationName, TKey key, TValue? value = null)
    {
        Name = operationName;
        Key = key;
        Value = value;
    }
    
    public override string ToString()
    {
        var valueString = Value is not null ? " Value: " + Value.ToListing() : "";
        return $"{Name} {TypeToString.GetEntityType(typeof(TValue))} {Key}." + valueString;
    }
    
    public enum OperationName
    {
        Create,
        Update,
        Delete,
        Execute
    }
}