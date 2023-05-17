using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public partial class CommandHandler<TKey, TValue> 
    where TValue : Entity<TKey> 
    where TKey : notnull
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
            feedbackEmitter.EmitSuccess(key, SystemOperation.Operation.Created);
            statusReport(manager.Get(key)!);
        }
        else
        {
            // Could not create entity
            feedbackEmitter.EmitFailure(key, SystemOperation.Operation.Created);
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
            feedbackEmitter.EmitSuccess(key, SystemOperation.Operation.Created);

            var shouldUpdate = typeof(TValue).GetProperties()
                .Where(pi => !pi.Name.Equals("Key"))
                .Select(pi => pi.GetValue(value))
                .Any(obj => obj is not null);

            if (shouldUpdate)
            {
                UpdateHandler(key, value, manager, feedbackEmitter, statusReport);
            }
            else
            {
                statusReport(manager.Get(key)!);
            }
        }
        else
        {
            // Could not create entity
            feedbackEmitter.EmitFailure(key, SystemOperation.Operation.Created);
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

        if (value.Key is not null && !key.Equals(value.Key) && manager.Get(value.Key) is not null)
        {
            // If the key is updated, it can only be updated to something that doesn't already exist
            feedbackEmitter.EmitAlreadyExists(value.Key!);
            return;
        }

        if (manager.Update(key, value))
        {
            feedbackEmitter.EmitSuccess(key, SystemOperation.Operation.Updated, value);
            statusReport(value.Key is not null ? manager.Get(value.Key)! : manager.Get(key)!);
        }
        else
        {
            feedbackEmitter.EmitFailure(key, SystemOperation.Operation.Updated, value);
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
            feedbackEmitter.EmitSuccess(key, SystemOperation.Operation.Deleted);
        }
        else
        {
            feedbackEmitter.EmitFailure(key, SystemOperation.Operation.Deleted);
        }
    }
    
    public static void ListHandler(IManager<TKey, TValue> manager)
    {
        var values = manager.GetAll().ToList();

        if (!values.Any())
        {
            return;
        }

        Console.WriteLine(values.First().ToListingHeader());
        values.Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
    }
    
    public static void StatusHandler(Action<TValue> statusAction, IManager<TKey, TValue> manager)
    {
        manager.GetAll().ToList().ForEach(statusAction);
    }
    
    public static void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IManager<TKey, TValue> manger1,
        IManager<TK, TV> manager2,
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
}