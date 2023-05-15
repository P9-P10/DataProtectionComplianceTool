using GraphManipulation.Commands;
using Xunit;

namespace Test.CLI;

public class CommandLineInterfaceTest
{
    // Create a factory for creating command handlers
    // Pass this factory to the Command Line Interface.
    // To test the CLI, test that it behaves correctly given a command string
    // this behaviour can include invoking handlers created by the factory
    // which can be verified by creating a factory producing test spies

    protected class MockComponentFactory : ComponentFactory
    {
        public MockComponentFactory(IManagerFactory managerFactory, IVacuumerFactory vacuumerFactory, ILoggerFactory loggerFactory, IConfigManagerFactory configManagerFactory) : base(managerFactory, vacuumerFactory, loggerFactory, configManagerFactory)
        {
        }
    }
    
    [Fact]
    public void test()
    {
        
    }
}