using System;
using System.Collections.Generic;
using GraphManipulation.Commands;
using GraphManipulation.Commands.Factories;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;
using GraphManipulation.Vacuuming;
using Xunit;

namespace Test.CLI;

public class CommandLineInterfaceTest
{
    // Create a factory for creating command handlers
    // Pass this factory to the Command Line Interface.
    // To test the CLI, test that it behaves correctly given a command string
    // this behaviour can include invoking handlers created by the factory
    // which can be verified by creating a factory producing test spies

    protected class SpyHandler<K,V> : IHandler<K,V> where V : Entity<K>
    {
        public void CreateHandler(K key, V value)
        {
            throw new NotImplementedException();
        }

        public void UpdateHandler(K key, V value)
        {
            throw new NotImplementedException();
        }

        public void DeleteHandler(K key)
        {
            throw new NotImplementedException();
        }

        public void ShowHandler(K key)
        {
            throw new NotImplementedException();
        }

        public void ListHandler()
        {
            throw new NotImplementedException();
        }

        public void StatusHandler()
        {
            throw new NotImplementedException();
        }

        public void ListChangesHandler<TK, TV>(K key, IEnumerable<TK> list, Func<V, IEnumerable<TV>> getCurrentList, Action<V, IEnumerable<TV>> setList, bool isAdd,
            IGetter<TV, TK> manager) where TV : Entity<TK>
        {
            throw new NotImplementedException();
        }
    }

    protected class SpyHandlerFactory : IHandlerFactory
    {
        public IHandler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TK : notnull where TV : Entity<TK>, new()
        {
            throw new NotImplementedException();
        }
    }
    
    
    protected class ManagerSpy<K,V> : IManager<K,V>
    {
        public IEnumerable<V> GetAll()
        {
            throw new NotImplementedException();
        }

        public V? Get(K key)
        {
            throw new NotImplementedException();
        }

        public bool Create(K key)
        {
            throw new NotImplementedException();
        }

        public bool Update(K key, V value)
        {
            throw new NotImplementedException();
        }

        public bool Delete(K key)
        {
            throw new NotImplementedException();
        }
    }

    protected class SpyManagerFactory : IManagerFactory
    {
        public IManager<TK, TV> CreateManager<TK, TV>() where TK : notnull where TV : Entity<TK>, new()
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void test()
    {
        
    }
}