using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Utility;

public interface ICommandHandler<TKey, TValue> where TValue : Entity<TKey> where TKey : notnull
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
        IManager<TK, TV> manager)
        where TV : Entity<TK>;
}

public partial class CommandHandler<TKey, TValue> : ICommandHandler<TKey, TValue>
    where TValue : Entity<TKey> where TKey : notnull
{
    private readonly IManager<TKey, TValue> _manager;
    private readonly FeedbackEmitter<TKey, TValue> _feedbackEmitter;
    private readonly Action<TValue> _statusReport;

    public CommandHandler(IManager<TKey, TValue> manager, FeedbackEmitter<TKey, TValue> feedbackEmitter,
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

    public void CreateHandler(TKey key, TValue value)
    {
        CreateHandler(key, value, _manager, _feedbackEmitter, _statusReport);
    }

    public void UpdateHandler(TKey key, TValue value)
    {
        UpdateHandler(key, value, _manager, _feedbackEmitter, _statusReport);
    }

    public void DeleteHandler(TKey key)
    {
        DeleteHandler(key, _manager, _feedbackEmitter);
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
    
    public void StatusHandler()
    {
        StatusHandler(_statusReport, _manager);
    }
    
    public void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IManager<TK, TV> manager)
        where TV : Entity<TK>
    {
        ListChangesHandler(key, list, getCurrentList, setList, isAdd, _manager, manager, _feedbackEmitter,
            new FeedbackEmitter<TK, TV>(), _statusReport);
    }
}