﻿using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Decorators;

public class LoggingManager<TKey, TValue> : LoggingDecorator<TKey, TValue>, IManager<TKey, TValue> where TValue : Entity<TKey>
{
    private readonly IManager<TKey, TValue> _manager;

    public LoggingManager(IManager<TKey, TValue> manager, ILogger logger) : base(logger)
    {
        _manager = manager;
    }

    public IEnumerable<TValue> GetAll()
    {
        return _manager.GetAll();
    }

    public TValue? Get(TKey key)
    {
        return _manager.Get(key);
    }

    public bool Create(TKey key)
    {
        var success = _manager.Create(key);
        
        if (success)
        {
            LogCreate(key);
        }

        return success;
    }

    public bool Update(TKey key, TValue value)
    {
        var success = _manager.Update(key, value);
        
        if (success)
        {
            LogUpdate(key, value);
        }

        return success;
    }

    public bool Delete(TKey key)
    {
        var success = _manager.Delete(key);
        
        if (success)
        {
            LogDelete(key);
        }

        return success;
    }
}