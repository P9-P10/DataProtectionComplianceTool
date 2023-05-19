using System.Runtime.InteropServices;

namespace IntegrationTests.SystemTest.Tools;

public sealed class WindowsFact : FactAttribute
{
    public WindowsFact() {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Skip = "Ignore on Linux";
        }
    }
}

public sealed class LinuxFact : FactAttribute
{
    public LinuxFact() {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            Skip = "Ignore on Windows";
        }
    }
}