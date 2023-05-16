using System;
using System.Collections.Generic;
using GraphManipulation.Commands;
using GraphManipulation.Commands.Factories;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;
using Moq;
using Xunit;

namespace Test.CLI;

public class MockManagerFactory : IManagerFactory
{
    public Dictionary<Type, Mock> Mocks { get; set; }

    public MockManagerFactory()
    {
        Mocks = new Dictionary<Type, Mock>();
    }

    public void AddMockManagerForType(Type type, Mock mock)
    {
        Mocks.Add(type, mock);
    }

    public void AddMockManager<T>(Mock mock)
    {
        AddMockManagerForType(typeof(T), mock);
    }
    
    public IManager<TK, TV> CreateManager<TK, TV>() where TK : notnull where TV : Entity<TK>, new()
    {
        if (Mocks.ContainsKey(typeof(TV)))
            return Mocks[typeof(TV)].Object as IManager<TK, TV>;
        var mock = new Mock<IManager<TK, TV>>();
        Mocks.Add(typeof(TV), mock);
        return mock.Object;
    }
}

public class CommandLineInterfaceTest
{
    [Fact]
    public void DoesNotCreateAlreadyExistingEntity()
    {
        var factory = new MockManagerFactory();
        
        var manager = new Mock<IManager<string, StorageRule>>();
        manager.Setup(manager => manager.Get("testname")).Returns(new StorageRule());
        
        factory.AddMockManager<StorageRule>(manager);

        CommandLineInterface cli = new CommandLineInterface(factory);
        cli.Invoke($"dc c -n testname");
        
        manager.Verify(manager => manager.Get(
            It.Is<string>(s => s == "testname")));
        manager.Verify(manager => manager.Create(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public void CreateCommandCreatesANewDeleteCondition()
    {
        var factory = new MockManagerFactory();
        
        var manager = new Mock<IManager<string, StorageRule>>();
        manager.Setup(manager => manager.Get("testname")).Returns((StorageRule?)null);
        
        factory.AddMockManager<StorageRule>(manager);

        CommandLineInterface cli = new CommandLineInterface(factory);
        cli.Invoke($"dc c -n testname");
        
        manager.Verify(manager => manager.Create(It.Is<string>(s => s == "testname")));
    }
    
    [Fact]
    public void CreatedEntityIsUpdatedWithAdditionalInformation()
    {
        var factory = new MockManagerFactory();
        
        var manager = new Mock<IManager<string, StorageRule>>();
        manager.Setup(manager => manager.Create(It.IsAny<string>())).Returns(true);
        
        manager.SetupSequence(manager => manager.Get(It.Is<string>(s => s == "testname")))
            .Returns(() => null)
            .Returns(new StorageRule());
        
        factory.AddMockManager<StorageRule>(manager);

        CommandLineInterface cli = new CommandLineInterface(factory);
        cli.Invoke($"dc c -n testname -d description");
        
        manager.Verify(manager => 
            manager.Update(
                It.Is<string>(s => s == "testname"),
                It.Is<StorageRule>(rule => rule.Description == "description")));
    }
}