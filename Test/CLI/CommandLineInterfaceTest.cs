using System;
using GraphManipulation.Commands;
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

    protected class MockComponentFactory : IComponentFactory
    {
        public IManager<TK, TV> CreateManager<TK, TV>() where TK : notnull where TV : Entity<TK>, new()
        {
            throw new NotImplementedException();
        }

        public IVacuumer CreateVacuumer()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger()
        {
            throw new NotImplementedException();
        }

        public IConfigManager CreateConfigManager()
        {
            throw new NotImplementedException();
        }

        public Handler<TK, TV> CreateHandler<TK, TV>(FeedbackEmitter<TK, TV> emitter, Action<TV> statusReport) where TK : notnull where TV : Entity<TK>, new()
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void test()
    {
        
    }
}