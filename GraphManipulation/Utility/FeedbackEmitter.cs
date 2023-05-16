using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public class FeedbackEmitter<TKey, TValue> where TValue : Entity<TKey>
{
    public void EmitSuccess(TKey key, SystemOperation.Operation operation, TValue? value = null)
    {
        Console.WriteLine(SuccessMessage(key, operation, value));
    }
    // TODO: Value should be the complete new value instead of what is just updated
    public static string SuccessMessage(TKey key, SystemOperation.Operation operation, TValue? value)
    {
        return ResultMessage(key, operation, true, value);
    }

    public void EmitFailure(TKey key, SystemOperation.Operation operation, TValue? value = null)
    {
        Console.Error.WriteLine(FailureMessage(key, operation, value));
    }

    public static string FailureMessage(TKey key, SystemOperation.Operation operation, TValue? value)
    {
        return ResultMessage(key, operation, false, value);
    }

    private static string ResultMessage(TKey key, SystemOperation.Operation operation, bool isSuccess, TValue? value)
    {
        return $"{TypeToString.GetEntityType(typeof(TValue)).FirstCharToUpper()} '{key}' " +
               (isSuccess ? "successfully " : "could not be ") +
               SystemOperation.OperationToString(operation) +
               (value is not null ? $" with {value.ToListing()}" : "");
    }

    public void EmitAlreadyExists(TKey key)
    {
        Console.Error.WriteLine(AlreadyExistsMessage(key));
    }

    public static string AlreadyExistsMessage(TKey key)
    {
        return $"Found an existing {TypeToString.GetEntityType(typeof(TValue))} using '{key}'";
    }

    public void EmitCouldNotFind(TKey key)
    {
        Console.Error.WriteLine(CouldNotFindMessage(key));
    }

    public static string CouldNotFindMessage(TKey key)
    {
        return $"Could not find {TypeToString.GetEntityType(typeof(TValue))} using '{key}'";
    }

    public void EmitMissing<TK, TV>(TKey subject)
        where TV : Entity<TK>
    {
        Console.WriteLine(MissingMessage<TK, TV>(subject));
    }

    public void EmitMissing(TKey subject, string obj)
    {
        Console.WriteLine(MissingMessage(subject, obj));
    }

    public static string MissingMessage<TK, TV>(TKey subject)
    where TV : Entity<TK>
    {
        return MissingMessage(subject, TypeToString.GetEntityType(typeof(TV)));
    }

    public static string MissingMessage(TKey subject, string obj)
    {
        return $"{TypeToString.GetEntityType(typeof(TValue)).FirstCharToUpper()} '{subject}' is missing {AOrAn(obj)} {obj}";
    }

    private static string AOrAn(string obj)
    {
        return new List<char> { 'a', 'e', 'i', 'o', 'u' }.Contains(char.ToLower(obj[0])) ? "an" : "a";
    }
}