using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Models;

namespace GraphManipulation.Logging.Operations;

public abstract class Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public SystemAction.Operation OperationName { get; } // The name of the operation
    public TKey Key { get; } // The identifier of the subject
    public TValue? Value { get; protected set; }

    public Operation(SystemAction.Operation operationName, TKey key, TValue? value = null)
    {
        OperationName = operationName;
        Key = key;
        Value = value;
    }
    
    public override string ToString()
    {
        var valueString = Value is not null ? " Value: " + Value.ToListing() : "";
        return $"{TypeToString.GetEntityType(typeof(TValue)).FirstCharToUpper()} '{Key}' {SystemAction.OperationToString(OperationName)}." + valueString;
    }
}