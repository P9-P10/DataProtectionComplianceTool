using GraphManipulation.Helpers;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Helpers;

public class FeedbackEmitter<TKey, TValue> where TValue : Entity<TKey>
{
    public void EmitSuccess(TKey key, Operations operation, TValue? value = null)
    {
        Console.WriteLine(SuccessMessage(key, operation, value));
    }
    
    private static string SuccessMessage(TKey key, Operations operation, TValue? value)
    {
        return $"Successfully {OperationToString(operation)} {key} {TypeToString.GetEntityType(typeof(TValue))}" +
               (value is not null ? $" with {value.ToListing()}" : "");
    }

    public void EmitFailure(TKey key, Operations operation, TValue? value = null)
    {
        Console.Error.WriteLine(FailureMessage(key, operation, value));
    }
    
    private static string FailureMessage(TKey key, Operations operation, TValue? value)
    {
        return $"{key} {TypeToString.GetEntityType(typeof(TValue))} could not be {OperationToString(operation)}" +
               (value is not null ? $" with {value.ToListing()}" : "");
    }

    public void EmitAlreadyExists(TKey key)
    {
        Console.Error.WriteLine(AlreadyExistsMessage(key));
    }
    
    private static string AlreadyExistsMessage(TKey key)
    {
        return $"Found an existing {TypeToString.GetEntityType(typeof(TValue))} using {key}";
    }

    public void EmitCouldNotFind(TKey key)
    {
        Console.Error.WriteLine(CouldNotFindMessage(key));
    }
    
    private static string CouldNotFindMessage(TKey key)
    {
        return $"Could not find {TypeToString.GetEntityType(typeof(TValue))} using {key}";
    }

    public void EmitMissing<TK, TV>(TKey subject)
        where TV: Entity<TK>
    {
        Console.WriteLine(MissingMessage<TKey, TValue>(subject, TypeToString.GetEntityType(typeof(TV))));
    }

    public void EmitMissing(TKey subject, string obj)
    {
        Console.WriteLine(MissingMessage<TKey, TValue>(subject, obj));
    }

    private static string MissingMessage<TK, TV>(TK subject, string obj)
        where TV : Entity<TK>
    {
        return $"{subject} {TypeToString.GetEntityType(typeof(TV))} is missing a(n) {obj}";
    }

    private static string OperationToString(Operations operation)
    {
        return operation.ToString().ToLower();
    }
    
    public enum Operations
    {
        Updated,
        Deleted,
        Created,
        Set,
        Removed,
        Executed
    }
}