using System;
using System.Collections.Generic;
using GraphManipulation.Commands;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using GraphManipulation.Vacuuming;
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

public class MockLoggerFactory : ILoggerFactory
{
    public ILogger CreateLogger()
    {
        return Mock.Of<ILogger>();
    }
}

public class MockVacuumerFactory : IVacuumerFactory
{
    public IVacuumer CreateVacuumer()
    {
        return Mock.Of<IVacuumer>();
    }
}

public class CommandLineInterfaceTest
{
    [Fact]
    public void DoesNotCreateAlreadyExistingEntity()
    {
        var managerFactory = new MockManagerFactory();
        var loggerFactory = new MockLoggerFactory();
        var vacuumerFactory = new MockVacuumerFactory();

        var manager = new Mock<IManager<string, StoragePolicy>>();
        manager.Setup(manager => manager.Get("testname")).Returns(new StoragePolicy());
        
        managerFactory.AddMockManager<StoragePolicy>(manager);
        
        CommandLineInterface cli = new CommandLineInterface(managerFactory, loggerFactory, vacuumerFactory);
        cli.Invoke($"{CommandNamer.StoragePolicyName} c -n testname");

        manager.Verify(manager => manager.Get(
            It.Is<string>(s => s == "testname")));
        manager.Verify(manager => manager.Create(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public void CreateCommandCreatesANewDeleteCondition()
    {
        var managerFactory = new MockManagerFactory();
        var loggerFactory = new MockLoggerFactory();
        var vacuumerFactory = new MockVacuumerFactory();
        
        var manager = new Mock<IManager<string, StoragePolicy>>();
        manager.Setup(manager => manager.Get("testname")).Returns((StoragePolicy?)null);
        
        managerFactory.AddMockManager<StoragePolicy>(manager);
        
        CommandLineInterface cli = new CommandLineInterface(managerFactory, loggerFactory, vacuumerFactory);
        cli.Invoke($"{CommandNamer.StoragePolicyName} c -n testname");

        manager.Verify(manager => manager.Create(It.Is<string>(s => s == "testname")));
    }
    
    [Fact]
    public void CreatedEntityIsUpdatedWithAdditionalInformation()
    {
        var managerFactory = new MockManagerFactory();
        var loggerFactory = new MockLoggerFactory();
        var vacuumerFactory = new MockVacuumerFactory();
        
        var manager = new Mock<IManager<string, StoragePolicy>>();
        manager.Setup(manager => manager.Create(It.IsAny<string>())).Returns(true);
        
        manager.SetupSequence(manager => manager.Get(It.Is<string>(s => s == "testname")))
            .Returns(() => null)
            .Returns(new StoragePolicy());
        
        managerFactory.AddMockManager<StoragePolicy>(manager);
        
        CommandLineInterface cli = new CommandLineInterface(managerFactory, loggerFactory, vacuumerFactory);
        cli.Invoke($"{CommandNamer.StoragePolicyName} c -n testname -d description");

        manager.Verify(manager => 
            manager.Update(
                It.Is<string>(s => s == "testname"),
                It.Is<StoragePolicy>(storagePolicy => storagePolicy.Description == "description")));
    }
}