using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Test.SystemTest;

public class CommandlineTest
{
    [Fact]
    public void TestCommandLine()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        
        process.GiveInput("help");

        string result = process.GetOutput();
        result.Should().Be(@$"Using config found at {SystemTest.DefaultConfigPath}" +
                           "Description:  This is a description of the root command" +
                           "Usage:  ! [command] [options]" +
                           "Options:  --version       " +
                           "Show version information  -?, -h, --help  Show help and usage information" +
                           "Commands:  " +
                           "ids, individuals  " +
                           "pd, personal-data  " +
                           "ps, purposes  " +
                           "origins, os  " +
                           "vacuuming-rules, vrs  " +
                           "dcs, delete-conditions  " +
                           "processings, prs  " +
                           "lgs, logs  " +
                           "cfg, configuration");
    }

    [Fact]
    public void AddFirstPurposeToColumn()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput(@"pd a -tc users email -jc """" -p NewName");

        string result = process.GetOutput();
        result.Should().Be(@$"Using config found at {SystemTest.DefaultConfigPath}");

        string error = process.GetError();
        error.Should().NotBeEmpty();
    }

    // These tests are to confirm that it is not possible to do system test by redirecting stdin and stdout
    // when using sharprompt
    [Fact]
    public void TestRedirectConsoleOutput()
    {
        FileStream stream = new FileStream("ConsoleTest.txt", FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);

        stream.Length.Should().Be(0);

        Console.SetOut(writer);
        Console.WriteLine("test");
        writer.Close();

        StreamReader reader = new StreamReader("ConsoleTest.txt");

        reader.ReadToEnd().Should().Be("test" + Environment.NewLine);

        Console.IsOutputRedirected.Should().BeTrue();
    }
}