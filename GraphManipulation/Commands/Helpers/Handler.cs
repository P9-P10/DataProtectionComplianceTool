using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Helpers;

public interface IHandler<TKey, TValue> where TValue : Entity<TKey> where TKey : notnull
{
    public void CreateHandler(TKey key, TValue value);
    public void UpdateHandler(TKey key, TValue value);
    public void DeleteHandler(TKey key);
    public void ShowHandler(TKey key);
    public void ListHandler();
    public void StatusHandler();

    public void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IGetter<TV, TK> manager)
        where TV : Entity<TK>;
}

public class Handler<TKey, TValue> : IHandler<TKey, TValue> where TValue : Entity<TKey> where TKey : notnull
{
    private readonly IManager<TKey, TValue> _manager;
    private readonly FeedbackEmitter<TKey, TValue> _feedbackEmitter;
    private readonly Action<TValue> _statusReport;

    public Handler(IManager<TKey, TValue> manager, FeedbackEmitter<TKey, TValue> feedbackEmitter,
        Action<TValue> statusReport)
    {
        _feedbackEmitter = feedbackEmitter;
        _statusReport = statusReport;
        _manager = manager;
    }

    public void CreateHandler(TKey key)
    {
        CreateHandler(key, _manager, _feedbackEmitter, _statusReport);
    }

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

    public void CreateHandler(TKey key, TValue value)
    {
        CreateHandler(key, value, _manager, _feedbackEmitter, _statusReport);
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
            feedbackEmitter.EmitFailure(key, FeedbackEmitter<TKey, TValue>.Operations.Created);
        }
    }

    public void UpdateHandler(TKey key, TValue value)
    {
        UpdateHandler(key, value, _manager, _feedbackEmitter, _statusReport);
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
            feedbackEmitter.EmitSuccess(key, FeedbackEmitter<TKey, TValue>.Operations.Updated, value);
            statusReport(value.Key is not null ? manager.Get(value.Key)! : manager.Get(key)!);
        }
        else
        {
            feedbackEmitter.EmitFailure(key, FeedbackEmitter<TKey, TValue>.Operations.Updated, value);
        }
    }

    public void DeleteHandler(TKey key)
    {
        DeleteHandler(key, _manager, _feedbackEmitter);
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

    public void ShowHandler(TKey key)
    {
        ShowHandler(key, _manager, _feedbackEmitter);
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

    public void ListHandler()
    {
        ListHandler(_manager);
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

    public void StatusHandler()
    {
        StatusHandler(_statusReport, _manager);
    }

    public static void StatusHandler(Action<TValue> statusAction, IManager<TKey, TValue> manager)
    {
        manager.GetAll().ToList().ForEach(statusAction);
    }

    public void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IGetter<TV, TK> manager)
        where TV : Entity<TK>
    {
        ListChangesHandler(key, list, getCurrentList, setList, isAdd, _manager, manager, _feedbackEmitter,
            new FeedbackEmitter<TK, TV>(), _statusReport);
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
}