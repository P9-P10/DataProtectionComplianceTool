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

    private class TestProcess : IDisposable
    {
        private readonly string _executablePath;
        public Process Process { get; private set; }
        public List<string> Output { get; private set; }

        public TestProcess(string executablePath)
        {
            _executablePath = executablePath;
            Process = CreateProcess(executablePath);
            Output = new List<string>();
        }
        
        private Process CreateProcess(string executablePath)
        {
            ProcessStartInfo startInfo = CreateProcessStartInfo(executablePath);
            Process process = new Process();
            process.StartInfo = startInfo;

            return process;
        }
        
        private ProcessStartInfo CreateProcessStartInfo(string executablePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;

            startInfo.UseShellExecute = false;
            startInfo.Arguments = "";
            startInfo.FileName = executablePath;

            return startInfo;
        }

        public void Start()
        {
            Process.OutputDataReceived += (sender, args) => Output.Add(args.Data);
            Process.Start();
            Process.BeginOutputReadLine();
        }

        public void GiveInput(string input)
        {
            Process.StandardInput.WriteLine(input);
            Process.WaitForExit(1000);
        }

        public string GetOutput()
        {
            return string.Join("", Output);
        }

        public void Dispose()
        {
            Process.Kill();
            Process.Dispose();
        }
    }


    private string CreateConfigFile()
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

        return configFilePath;
    }
    
    [Fact]
    public void TestCommandLine()
    {
        string configFilePath = CreateConfigFile();
        string executablePath = Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe");

        TestProcess process = new TestProcess(executablePath);
        try
        {
            process.Start();
            process.GiveInput("--help");

            string result = process.GetOutput();
            result.Should().Be(@$"Using config found at {configFilePath}Description:  This is a description of the root commandUsage:  ! [command] [options]Options:  --version       Show version information  -?, -h, --help  Show help and usage informationCommands:  ids, individuals  pd, personal-data  ps, purposes  origins, os  vacuuming-rules, vrs  dcs, delete-conditions  processings, prs  lgs, logs  cfg, configuration");
        }
        finally
        {
            process.Dispose();
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