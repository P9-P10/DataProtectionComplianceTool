using System.Collections.Generic;
using GraphManipulation.Commands;
using GraphManipulation.Commands.Factories;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;
using Moq;
using Xunit;

namespace Test.CLI;

public class CommandLineInterfaceTest
{
    // Create a factory for creating command handlers
    // Pass this factory to the Command Line Interface.
    // To test the CLI, test that it behaves correctly given a command string
    // this behaviour can include invoking handlers created by the factory
    // which can be verified by creating a factory producing test spies

    protected class ManagerSpy<K,V> : IManager<K,V> where V : class
    {
        public IEnumerable<V> GetAll()
        {
            return new List<V>();
        }

        public V? Get(K key)
        {
            return null;
        }

        public bool Create(K key)
        {
            return true;
        }

        public bool Update(K key, V value)
        {
            return true;
        }

        public bool Delete(K key)
        {
            return true;
        }
    }
    
    protected class SpyManagerFactory<K, V> : IManagerFactory
    {
        public Mock<IManager<K, V>> ManagerMock { get; private set; }

        public SpyManagerFactory(Mock<IManager<K, V>> mockManager)
        {
            ManagerMock = mockManager;
        }
        public IManager<TK, TV> CreateManager<TK, TV>() where TK : notnull where TV : Entity<TK>, new()
        {
            var manager = new ManagerSpy<TK, TV>();
            if (typeof(TK) == typeof(K) && typeof(TV) == typeof(V))
                return ManagerMock.Object as IManager<TK, TV>;
            return manager;
        }
    }

    [Fact]
    public void CreateCallsGet()
    {
        var manager = new Mock<IManager<string, StorageRule>>();
        var factory = new SpyManagerFactory<string, StorageRule>(manager);

        CommandLineInterface cli = new CommandLineInterface(factory);
        cli.Invoke($"dc c -n testname");
        
        manager.Verify(manager => manager.Get(
            It.Is<string>(s => s == "testname")));
    }
    
    [Fact]
    public void CreateCallsCreate()
    {
        var manager = new Mock<IManager<string, StorageRule>>();
        var factory = new SpyManagerFactory<string, StorageRule>(manager);

        CommandLineInterface cli = new CommandLineInterface(factory);
        cli.Invoke($"dc c -n testname");
        
        manager.Verify(manager => manager.Create(It.Is<string>(s => s == "testname")));
    }
    
    [Fact]
    public void CreateWithDescriptionCallsUpdate()
    {
        var manager = new Mock<IManager<string, StorageRule>>();
        manager.Setup(manager => manager.Create(It.IsAny<string>())).Returns(true);
        
        manager.SetupSequence(manager => manager.Get(It.Is<string>(s => s == "testname")))
            .Returns(() => null)
            .Returns(new StorageRule());
        
        var factory = new SpyManagerFactory<string, StorageRule>(manager);

        CommandLineInterface cli = new CommandLineInterface(factory);
        cli.Invoke($"dc c -n testname -d description");
        
        manager.Verify(manager => 
            manager.Update(
                It.Is<string>(s => s == "testname"),
                It.Is<StorageRule>(rule => rule.Description == "description")));
    }
}