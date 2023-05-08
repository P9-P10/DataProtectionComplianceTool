using System.Runtime.InteropServices;

namespace IntegrationTests.SystemTest.Tools.Test;

public class PlatformFactAttributesTest
{
    [WindowsFact]
    public void WindowsFactOnlyRunsOnWindows()
    {
        Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
    }

    [LinuxFact]
    public void LinuxFactOnlyRunsOnLinux()
    {
        Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
    }
}