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

    public ILineReader Reader();

    public void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IManager<TK, TV> manager)
        where TV : Entity<TK>;
}

public class CommandHandler<TKey, TValue> : ICommandHandler<TKey, TValue>
    where TValue : Entity<TKey> where TKey : notnull
{
    private readonly IManager<TKey, TValue> _manager;
    private readonly FeedbackEmitter<TKey, TValue> _feedbackEmitter;
    private readonly Action<TValue> _statusReport;
    private readonly ILineReader _reader;

    public CommandHandler(IManager<TKey, TValue> manager, FeedbackEmitter<TKey, TValue> feedbackEmitter,
        Action<TValue> statusReport, ILineReader reader)
    {
        _feedbackEmitter = feedbackEmitter;
        _statusReport = statusReport;
        _reader = reader;
        _manager = manager;
    }

    public void CreateHandler(TKey key)
    {
        Handlers.CreateHandler(key, _manager, _feedbackEmitter, _statusReport);
    }

    public ILineReader Reader()
    {
        return _reader;
    }
    public void CreateHandler(TKey key, TValue value)
    {
        Handlers.CreateHandler(key, value, _manager, _feedbackEmitter, _statusReport);
    }

    public void UpdateHandler(TKey key, TValue value)
    {
        Handlers.UpdateHandler(key, value, _manager, _feedbackEmitter, _statusReport);
    }

    public void DeleteHandler(TKey key)
    {
        Handlers.DeleteHandler(key, _manager, _feedbackEmitter);
    }

    public void ShowHandler(TKey key)
    {
        Handlers.ShowHandler(key, _manager, _feedbackEmitter);
    }

    public void ListHandler()
    {
        Handlers.ListHandler(_manager);
    }

    public void StatusHandler()
    {
        Handlers.StatusHandler(_statusReport, _manager);
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
        Handlers.ListChangesHandler(key, list, getCurrentList, setList, isAdd, _manager, manager, _feedbackEmitter,
            new FeedbackEmitter<TK, TV>(), _statusReport, _reader);
    }
}