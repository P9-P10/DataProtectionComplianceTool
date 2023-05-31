using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class QuitCommandTest : TestResources
{
    [Fact]
    public void ClosesProgram()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        QuitProgram(process);
        Thread.Sleep(1000);

        process.Process.HasExited.Should().BeTrue();
        process.Process.ExitCode.Should().Be(0);
    }
}