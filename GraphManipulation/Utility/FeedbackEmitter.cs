using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public class FeedbackEmitter<TKey, TValue> where TValue : Entity<TKey>
{
    public void EmitSuccess(TKey key, SystemOperation.Operation operation, TValue? value = null)
    {
        Console.WriteLine(FeedbackEmitterMessage.SuccessMessage(key, operation, value));
    }

    public void EmitFailure(TKey key, SystemOperation.Operation operation, TValue? value = null)
    {
        Console.Error.WriteLine(FeedbackEmitterMessage.FailureMessage(key, operation, value));
    }

    public void EmitAlreadyExists(TKey key)
    {
        Console.Error.WriteLine(FeedbackEmitterMessage.AlreadyExistsMessage<TKey, TValue>(key));
    }

    public void EmitCouldNotFind(TKey key)
    {
        Console.Error.WriteLine(FeedbackEmitterMessage.CouldNotFindMessage<TKey, TValue>(key));
    }

    public void EmitMissing<TValueOther>(TKey subject)
    {
        Console.WriteLine(FeedbackEmitterMessage.MissingMessage<TKey, TValue, TValueOther>(subject));
    }

    public void EmitMissing(TKey subject, string obj)
    {
        Console.WriteLine(FeedbackEmitterMessage.MissingMessage<TKey, TValue>(subject, obj));
    }
}