using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Helpers;

public static class Handlers<TKey, TValue> where TValue : Entity<TKey>
{
    public static void CreateHandler(TKey key, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter, Action<TValue> statusReport)
    {
        if (manager.Get(key) is not null)
        {
            // Cannot create something that exists already
            feedbackEmitter.EmitAlreadyExists(key);
            return;
        }

        if (manager.Create(key))
        {
            feedbackEmitter.EmitSuccess(key, FeedbackEmitter<TKey, TValue>.Operations.Created);
            statusReport(manager.Get(key)!);
        }
        else
        {
            // Could not create entity
            feedbackEmitter.EmitFailure(key, FeedbackEmitter<TKey, TValue>.Operations.Created);
        }
    }

    public static void CreateHandler(TKey key, TValue value, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter, Action<TValue> statusReport)
    {
        if (manager.Get(key) is not null)
        {
            // Cannot create something that exists already
            feedbackEmitter.EmitAlreadyExists(key);
            return;
        }

        if (manager.Create(key))
        {
            feedbackEmitter.EmitSuccess(key, FeedbackEmitter<TKey, TValue>.Operations.Created);
            UpdateHandler(key, value, manager, feedbackEmitter, statusReport);
        }
        else
        {
            // Could not create entity
            feedbackEmitter.EmitFailure(key, FeedbackEmitter<TKey, TValue>.Operations.Created);
        }
    }

    public static void UpdateHandler(TKey key, TValue value, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter, Action<TValue> statusReport)
    {
        var old = manager.Get(key);

        if (old is null)
        {
            // Can only update something that exists
            feedbackEmitter.EmitCouldNotFind(key);
            return;
        }

        if (!key!.Equals(value.Key!) && manager.Get(value.Key!) is not null)
        {
            // If the key is updated, it can only be updated to something that doesn't already exist
            feedbackEmitter.EmitAlreadyExists(value.Key!);
            return;
        }

        old.Fill(value);

        if (manager.Update(key, value))
        {
            feedbackEmitter.EmitSuccess(key, FeedbackEmitter<TKey, TValue>.Operations.Updated, value);
            statusReport(manager.Get(key)!);
        }
        else
        {
            feedbackEmitter.EmitFailure(key, FeedbackEmitter<TKey, TValue>.Operations.Updated, value);
        }
    }

    public static void DeleteHandler(TKey key, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter)
    {
        if (manager.Get(key) is null)
        {
            // Can only delete something that exists
            feedbackEmitter.EmitCouldNotFind(key);
            return;
        }

        if (manager.Delete(key))
        {
            feedbackEmitter.EmitSuccess(key, FeedbackEmitter<TKey, TValue>.Operations.Deleted);
        }
        else
        {
            feedbackEmitter.EmitFailure(key, FeedbackEmitter<TKey, TValue>.Operations.Deleted);
        }
    }

    public static void ShowHandler(TKey key, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter)
    {
        if (manager.Get(key) is null)
        {
            // Can only show something that exists
            feedbackEmitter.EmitCouldNotFind(key);
            return;
        }

        Console.WriteLine(manager.Get(key)!.ToListing());
    }

    public static void ListHandler(IManager<TKey, TValue> manager)
    {
        manager.GetAll().Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
    }

    public static void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IManager<TKey, TValue> manger1,
        IGetter<TV, TK> manager2,
        FeedbackEmitter<TKey, TValue> feedbackEmitter1,
        FeedbackEmitter<TK, TV> feedbackEmitter2,
        Action<TValue> statusAction)
        where TV : Entity<TK>
    {
        var value = manger1.Get(key);
        if (value is null)
        {
            feedbackEmitter1.EmitCouldNotFind(key);
            return;
        }

        var currentList = getCurrentList(value).ToList();

        foreach (var k in list)
        {
            var entity = manager2.Get(k);

            if (entity is null)
            {
                feedbackEmitter2.EmitCouldNotFind(k);
                return;
            }

            if (isAdd)
            {
                if (!currentList.Contains(entity))
                {
                    currentList.Add(entity);
                }
            }
            else
            {
                if (currentList.Contains(entity))
                {
                    currentList.Remove(entity);
                }
            }
        }

        setList(value, currentList);
        UpdateHandler(key, value, manger1, feedbackEmitter1, statusAction);
    }

    public static void StatusHandler(Action<TValue> statusAction, IManager<TKey, TValue> manager)
    {
        manager.GetAll().ToList().ForEach(statusAction);
    }
}