using GraphManipulation.Helpers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Helpers;

public class FeedbackEmitter<TKey, TValue> where TValue : Entity<TKey>
{
    public void EmitSuccess(TKey key, SystemAction.Operation operation, TValue? value = null)
    {
        Console.WriteLine(SuccessMessage(key, operation, value));
    }
    // TODO: Value should be the complete new value instead of what is just updated
    private static string SuccessMessage(TKey key, SystemAction.Operation operation, TValue? value)
    {
        return ResultMessage(key, operation, true, value);
    }

    public void EmitFailure(TKey key, SystemAction.Operation operation, TValue? value = null)
    {
        Console.Error.WriteLine(FailureMessage(key, operation, value));
    }

    private static string FailureMessage(TKey key, SystemAction.Operation operation, TValue? value)
    {
        return ResultMessage(key, operation, false, value);
    }

    private static string ResultMessage(TKey key, SystemAction.Operation operation, bool isSuccess, TValue? value)
    {
        return $"{TypeToString.GetEntityType(typeof(TValue)).FirstCharToUpper()} '{key}' " +
               (isSuccess ? "successfully " : "could not be ") +
               SystemAction.OperationToString(operation) +
               (value is not null ? $" with {value.ToListing()}" : "");
    }

    public void EmitAlreadyExists(TKey key)
    {
        Console.Error.WriteLine(AlreadyExistsMessage(key));
    }

    private static string AlreadyExistsMessage(TKey key)
    {
        return $"Found an existing {TypeToString.GetEntityType(typeof(TValue))} using '{key}'";
    }

    public void EmitCouldNotFind(TKey key)
    {
        Console.Error.WriteLine(CouldNotFindMessage(key));
    }

    private static string CouldNotFindMessage(TKey key)
    {
        return $"Could not find {TypeToString.GetEntityType(typeof(TValue))} using '{key}'";
    }

    public void EmitMissing<TK, TV>(TKey subject)
        where TV : Entity<TK>
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
        return $"{TypeToString.GetEntityType(typeof(TV)).FirstCharToUpper()} '{subject}' is missing {AOrAn(obj)} {obj}";
    }

    private static string AOrAn(string obj)
    {
        return new List<char> { 'a', 'e', 'i', 'o', 'u' }.Contains(char.ToLower(obj[0])) ? "an" : "a";
    }
}