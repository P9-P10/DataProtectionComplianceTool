using GraphManipulation.Commands;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public static class Handlers
{
    public static void ShowHandler<TKey, TValue>(TKey key, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter) where TValue : Entity<TKey>
    {
        if (manager.Get(key) is null)
        {
            // Can only show something that exists
            feedbackEmitter.EmitCouldNotFind(key);
            return;
        }

        Console.WriteLine(manager.Get(key)!.ToListing());
    }

    public static void CreateHandler<TKey, TValue>(TKey key, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter, Action<TValue> statusReport) where TValue : Entity<TKey>
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

    public static void CreateHandler<TKey, TValue>(TKey key, TValue value, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter, Action<TValue> statusReport)
        where TValue : Entity<TKey> where TKey : notnull
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

    public static void UpdateHandler<TKey, TValue>(TKey key, TValue value, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter, Action<TValue> statusReport)
        where TValue : Entity<TKey> where TKey : notnull
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
            var newValue = value.Key is not null ? manager.Get(value.Key)! : manager.Get(key)!;
            feedbackEmitter.EmitSuccess(key, SystemOperation.Operation.Updated, newValue);
            statusReport(newValue);
        }
        else
        {
            feedbackEmitter.EmitFailure(key, SystemOperation.Operation.Updated);
        }
    }

    public static void DeleteHandler<TKey, TValue>(TKey key, IManager<TKey, TValue> manager,
        FeedbackEmitter<TKey, TValue> feedbackEmitter) where TValue : Entity<TKey>
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

    public static void ListHandler<TKey, TValue>(IManager<TKey, TValue> manager) where TValue : Entity<TKey>
    {
        var values = manager.GetAll().ToList();

        if (!values.Any())
        {
            return;
        }

        Console.WriteLine(values.First().ToListingHeader());
        values.Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
    }

    public static void StatusHandler<TValue, TKey>(Action<TValue> statusAction, IManager<TKey, TValue> manager)
        where TValue : Entity<TKey>
    {
        manager.GetAll().ToList().ForEach(statusAction);
    }

    public static void ListChangesHandler<TK, TV, TKey, TValue>(
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
        where TV : Entity<TK> where TValue : Entity<TKey> where TKey : notnull
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
                if (!PromptAndCreate(k, manager2))
                {
                    feedbackEmitter2.EmitCouldNotFind(k);
                    return;
                }

                entity = manager2.Get(k)!;
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

    public static IEnumerable<TValue> HandleMustExistList<TKey, TValue>(IEnumerable<TKey> keys, IManager<TKey, TValue> manager)
    {
        return keys.Select<TKey, TValue>(key => HandleMustExist(key, manager)).ToList();
    }

    public static TValue HandleMustExist<TKey, TValue>(TKey key, IManager<TKey, TValue> manager)
    {
        var value = manager.Get(key);
        if (value is not null)
        {
            return value;
        }

        throw new HandlerException($"Could not bind to {key} as it does not exist in the system");
    }

    public static IEnumerable<TValue> HandleMustExistListWithCreateOnDemand<TKey, TValue>(IEnumerable<TKey> keys,
        IManager<TKey, TValue> manager) where TValue : Entity<TKey> where TKey : notnull
    {
        return keys.Select<TKey, TValue>(key => HandleMustExistWithCreateOnDemand(key, manager)).ToList();
    }

    public static TValue HandleMustExistWithCreateOnDemand<TKey, TValue>(TKey key, IManager<TKey, TValue> manager)
        where TValue : Entity<TKey> where TKey : notnull
    {
        try
        {
            return HandleMustExist(key, manager);
        }
        catch (HandlerException)
        {
            if (!PromptAndCreate(key, manager))
            {
                throw;
            }
            
            return manager.Get(key)!;
        }
    }

    private static bool PromptAndCreate<TKey, TValue>(TKey key, IManager<TKey, TValue> manager)
        where TValue : Entity<TKey>
    {
        if (!PromptCreateNew<TKey, TValue>(key))
        {
            return false;
        }
        
        CreateHandler(key, manager, new FeedbackEmitter<TKey, TValue>(),
            _ => Console.WriteLine("Not reporting status when creating on demand"));
        return true;
    }

    private static bool PromptCreateNew<TKey, TValue>(TKey key)
        where TValue : Entity<TKey>
    {
        while (true)
        {
            Console.Write(
                $"{key} {TypeToString.GetEntityType(typeof(TValue))} does not exist. Would you like to create one? (y/n){Environment.NewLine}{CommandLineInterface.Prompt} ");
            var reply = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrEmpty(reply))
            {
                Console.WriteLine("You must answer either 'y' or 'n'");
            }
            else
                switch (reply)
                {
                    case "y" or "Y":
                        return true;
                    case "n" or "N":
                        return false;
                    default:
                        Console.WriteLine($"Cannot parse '{reply}', you must either answer 'y' or 'n'");
                        break;
                }
        }
    }
}

public class HandlerException : Exception
{
    public HandlerException(string message) : base(message)
    {
    }
}