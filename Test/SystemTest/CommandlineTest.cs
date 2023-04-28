using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using GraphManipulation;
using Newtonsoft.Json;
using Sharprompt;
using Xunit;

namespace Test.SystemTest;

public class CommandlineTest
{
    [Fact]
    public void TestCommandLine()
    {
        var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        var configValues = new Dictionary<string, string>
        {
            {"GraphStoragePath", "test"},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", "test"},
            {"LogPath", "system_test_log.txt"},
            {"DatabaseConnectionString", "system_test_db.sqlite"},
            {"IndividualsTable", "test"}
        };
        File.WriteAllText(configFilePath, JsonConvert.SerializeObject( configValues ));

        string executablePath = Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe");
        
        ProcessStartInfo startInfo = new ProcessStartInfo();
        Process p = new Process();

        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardInput = true;

        startInfo.UseShellExecute = false;
        startInfo.Arguments = "";
        startInfo.FileName = executablePath;

        try
        {
            p.StartInfo = startInfo;
            var output = new List<string>();
            p.OutputDataReceived += (sender, args) => output.Add(args.Data);
            p.Start();
            p.BeginOutputReadLine();
            p.StandardInput.WriteLine("--help");
            p.WaitForExit(1000);

            var result = string.Join("", output);

            result.Should().Be(@$"Using config found at {configFilePath}Description:  This is a description of the root commandUsage:  ! [command] [options]Options:  --version       Show version information  -?, -h, --help  Show help and usage informationCommands:  ids, individuals  pd, personal-data  ps, purposes  origins, os  vacuuming-rules, vrs  dcs, delete-conditions  processings, prs  lgs, logs  cfg, configuration");
        }
        finally
        {
            p.Kill();
        }
    }

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

    [Fact]
    public void RedirectingOutputCausesSharpromptToThrow()
    {
        // I believe that xunit redirects console output
        Action act = () => Prompt.Input<string>("");

        act.Should().Throw<TypeInitializationException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("Sharprompt requires an interactive environment.");

    }
}