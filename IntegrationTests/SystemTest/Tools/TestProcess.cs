using System.Collections;
using System.Diagnostics;
using System.Text;
using GraphManipulation;
using GraphManipulation.Commands;

namespace IntegrationTests.SystemTest.Tools;

public class TestProcess : IDisposable
{
    public TestProgram Process { get; }
    public List<string> Output { get; private set; }
    public List<string> Errors { get; private set; }

    private MemoryStream OutputStream;

    private MemoryStream ErrorStream;
    public List<string> Inputs { get; }

    public List<List<string>> AllOutputs { get; }
    public List<List<string>> AllErrors { get; }
    public string ConfigPath { get; }

    private bool processStarted = false;


    public TestProcess(string executablePath, string configPath = "")
    {
        AllOutputs = new List<List<string>>();
        AllErrors = new List<List<string>>();
        Inputs = new List<string>();
        ConfigPath = configPath;

        // Create memory stream and redirect console output
        OutputStream = new MemoryStream();
        StreamWriter outputWriter = new StreamWriter(OutputStream);
        outputWriter.AutoFlush = true;
        Console.SetOut(outputWriter);

        Console.SetIn(new TestReader($"y{Environment.NewLine}"));

        // Do the same for standard error
        ErrorStream = new MemoryStream();
        StreamWriter errorWriter = new StreamWriter(ErrorStream);
        errorWriter.AutoFlush = true;
        Console.SetError(errorWriter);


        Process = CreateProcess();
    }

    private class TestReader : TextReader
    {
        private readonly string _output;

        public TestReader(string output)
        {
            _output = output;
        }

        public override string ReadLine()
        {
            return _output;
        }
    }

    private TestProgram CreateProcess()
    {
        TestProgram process = new TestProgram();
        return process;
    }

    public void Start()
    {
        Process.Start(ConfigPath);
        processStarted = true;
    }

    public void GiveInput(string input)
    {
        Output = new List<string>();
        Errors = new List<string>();
        Inputs.Add(input);
        Process.Run(input);
        AwaitProcessResponse();
    }

    private string ReadStream(Stream stream)
    {
        var reader = new StreamReader(stream);
        stream.Position = 0;
        var output = reader.ReadToEnd();
        stream.SetLength(0);
        return output;
    }

    public string GetOutput()
    {
        return string.Join("", Output);
    }

    public List<string> GetAllOutput()
    {
        return FlattenList(AllOutputs);
    }

    public IEnumerable<string> GetAllOutputNoWhitespace()
    {
        return GetAllOutput()
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim());
    }

    public IEnumerable<string> GetAllOutputNoWhitespaceOrPrompt()
    {
        return GetAllOutput()
            .Select(s => s.Replace(CommandLineInterface.Prompt, ' '))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim());
    }

    public IEnumerable<string> GetLastOutputNoWhitespace()
    {
        return GetLastOutput().Where(s => !string.IsNullOrWhiteSpace(s));
    }

    public IEnumerable<string> GetLastOutputNoWhitespaceOrPrompt()
    {
        return GetLastOutput()
            .Select(s => s.Replace(CommandLineInterface.Prompt, ' '))
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim());
    }

    public List<string> GetLastOutput()
    {
        return Output;
    }

    public string GetError()
    {
        return string.Join(Environment.NewLine, Errors);
    }

    public List<string> GetAllErrors()
    {
        return FlattenList(AllErrors);
    }

    public IEnumerable<string> GetAllErrorsNoWhitespace()
    {
        return GetAllErrors().Where(s => !string.IsNullOrWhiteSpace(s));
    }

    public List<string> GetLastError()
    {
        return Errors;
    }

    private List<T> FlattenList<T>(List<List<T>> listOfLists)
    {
        return listOfLists.SelectMany(e => e).ToList();
    }

    private void AwaitProcessResponse()
    {
        string output = ReadStreamOutputToString();

        Output = output.Split(Environment.NewLine).ToList();

        AllOutputs.Add(Output);
        AllErrors.Add(Errors);
    }

    private string ReadStreamOutputToString()
    {
        return new string(ReadStream(OutputStream).ToArray());
    }


    public void Dispose()
    {
        if (processStarted)
        {
        }
    }
}