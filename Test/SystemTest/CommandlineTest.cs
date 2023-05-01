using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FluentAssertions;
using Newtonsoft.Json;
using Sharprompt;
using Xunit;

namespace Test.SystemTest;

public class CommandlineTest
{

    private class TestProcess : IDisposable
    {
        public Process Process { get; private set; }
        public List<string> Output { get; private set; }
        public List<string> Errors { get; private set; }

        private int producedOutput;
        private int producedError;

        public TestProcess(string executablePath)
        {
            Process = CreateProcess(executablePath);
            Output = new List<string>();
            Errors = new List<string>();
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
            startInfo.RedirectStandardError = true;

            startInfo.UseShellExecute = false;
            startInfo.Arguments = "";
            startInfo.FileName = executablePath;

            return startInfo;
        }

        public void Start()
        {
            Process.OutputDataReceived += (sender, args) =>
            {
                producedOutput++;
                Output.Add(args.Data);
            };
            Process.ErrorDataReceived += (sender, args) =>
            {
                producedError++;
                Errors.Add(args.Data);
            };
            Process.Start();
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
        }

        public void GiveInput(string input)
        {
            producedError = 0;
            producedOutput = 0;
            Process.StandardInput.WriteLine(input);

        }
        
        public string GetOutput()
        {
            int timeout = 100;
            int duration = 0;
            while (producedOutput < 20 && duration < timeout)
            {
                Thread.Sleep(10);
                duration++;
            }
            
            return string.Join("", Output);
        }
        
        public string GetError()
        {
            int timeout = 100;
            int duration = 0;
            while (producedError < 20 && duration < timeout)
            {
                Thread.Sleep(10);
                duration++;
            }
            return string.Join("\n", Errors);
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
            result.Should().Be(@$"Using config found at {configFilePath}" +
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
        finally
        {
            process.Dispose();
        }
    }

    [Fact]
    public void AddFirstPurposeToColumn()
    {
        string configFilePath = CreateConfigFile();
        string executablePath = Path.Combine(Directory.GetCurrentDirectory(), "GraphManipulation.exe");

        TestProcess process = new TestProcess(executablePath);
        try
        {
            process.Start();
            process.GiveInput(@"pd a -tc users email -jc """" -p NewName");
            string result = process.GetOutput();
            result.Should().Be(@$"Using config found at {configFilePath}");
            string error = process.GetError();
            process.Errors.Should().NotBeEmpty();
        }
        finally
        {
            process.Dispose();
        }
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

    [Fact]
    public void RedirectingOutputCausesSharpromptToThrow()
    {
        // I believe that xunit redirects console output
        // Therefore it is not necessary to setup redirection in this test
        Action act = () => Prompt.Input<string>("");

        act.Should().Throw<TypeInitializationException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("Sharprompt requires an interactive environment.");

    }
}