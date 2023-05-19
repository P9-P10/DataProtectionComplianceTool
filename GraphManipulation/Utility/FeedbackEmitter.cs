using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public class FeedbackEmitter<TKey, TValue> where TValue : Entity<TKey>
{
    public void EmitSuccess(TKey key, SystemOperation.Operation operation, TValue? value = null)
    {
        EmitMessage(FeedbackEmitterMessage.SuccessMessage(key, operation, value));
    }

    public void EmitFailure(TKey key, SystemOperation.Operation operation, TValue? value = null)
    {
        EmitError(FeedbackEmitterMessage.FailureMessage(key, operation, value));
    }

    public void EmitResult(TKey key, SystemOperation.Operation operation, TValue? value = null, bool? isSuccess = null)
    {
        EmitMessage(FeedbackEmitterMessage.ResultMessage(key, operation, isSuccess, value));
    }

    public void EmitAlreadyExists(TKey key)
    {
        EmitError(FeedbackEmitterMessage.AlreadyExistsMessage<TKey, TValue>(key));
    }

    public void EmitCouldNotFind(TKey key)
    {
        EmitError(FeedbackEmitterMessage.CouldNotFindMessage<TKey, TValue>(key));
    }

    public void EmitMissing<TValueOther>(TKey subject)
    {
        EmitMessage(FeedbackEmitterMessage.MissingMessage<TKey, TValue, TValueOther>(subject));
    }

    public void EmitMissing(TKey subject, string obj)
    {
        EmitMessage(FeedbackEmitterMessage.MissingMessage<TKey, TValue>(subject, obj));
    }

    public void EmitMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void EmitError(string error)
    {
        Console.Error.WriteLine(error);
    }
}