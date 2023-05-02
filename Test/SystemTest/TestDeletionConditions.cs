using FluentAssertions;
using Xunit;

namespace Test.SystemTest;

public class TestDeletionConditions
{
    [Fact]
    public void TestAddCommand()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput("dcs a -n DeletionCondition -c \"Condition\"");
        string result = process.GetOutput();
        string error = process.GetError();
        result.Should()
            .Be(
                $@"Using config found at {SystemTest.DefaultConfigPath}" +
                "$: Successfully added DeletionCondition entity with , Condition");
    }
}