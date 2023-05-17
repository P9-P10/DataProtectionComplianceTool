using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public class FeedbackEmitterMessage
{
    public static string SuccessMessage<TKey, TValue>(TKey key, SystemOperation.Operation operation, TValue? value) 
        where TValue : Entity<TKey>
    {
        return ResultMessage(key, operation, true, value);
    }

    public static string FailureMessage<TKey, TValue>(TKey key, SystemOperation.Operation operation, TValue? value) 
        where TValue : Entity<TKey>
    {
        return ResultMessage(key, operation, false, value);
    }

    private static string ResultMessage<TKey, TValue>(TKey key, SystemOperation.Operation operation, bool isSuccess, TValue? value) 
        where TValue : Entity<TKey>
    {
        return $"{TypeToString.GetEntityType(typeof(TValue)).FirstCharToUpper()} '{key}' " +
               (isSuccess ? "successfully " : "could not be ") +
               SystemOperation.OperationToString(operation) +
               (value is not null ? $" to '{value.ToListing()}'" : "");
    }

    public static string AlreadyExistsMessage<TKey, TValue>(TKey key) 
        where TValue : Entity<TKey>
    {
        return $"Found an existing {TypeToString.GetEntityType(typeof(TValue))} using '{key}'";
    }

    public static string CouldNotFindMessage<TKey, TValue>(TKey key)
        where TValue : Entity<TKey>
    {
        return $"Could not find {TypeToString.GetEntityType(typeof(TValue))} using '{key}'";
    }

    public static string MissingMessage<TKey, TValue, TValueOther>(TKey subject)
        where TValue : Entity<TKey>
    {
        return MissingMessage<TKey, TValue>(subject, TypeToString.GetEntityType(typeof(TValueOther)));
    }

    public static string MissingMessage<TKey, TValue>(TKey subject, string obj) 
        where TValue : Entity<TKey>
    {
        return $"{TypeToString.GetEntityType(typeof(TValue)).FirstCharToUpper()} '{subject}' is missing {AOrAn(obj)} {obj}";
    }

    private static string AOrAn(string obj)
    {
        return new List<char> { 'a', 'e', 'i', 'o', 'u' }.Contains(char.ToLower(obj[0])) ? "an" : "a";
    }
}