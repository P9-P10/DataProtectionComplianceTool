using FluentAssertions;

namespace IntegrationTests.SystemTest.Tools.Test;

public class SystemTestToolsTest
{
    [Fact]
    public void CreateTestProcessRemovesSpacesInFilePaths()
    {
        using TestProcess process = SystemTest.CreateTestProcess("testCallerName", "spa ces");
        process.ConfigPath.Should().Contain("spaces");
        process.ConfigPath.Should().NotContain(" ");
    }
}