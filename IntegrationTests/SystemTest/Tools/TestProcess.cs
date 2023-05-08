using System.Diagnostics;

namespace IntegrationTests.SystemTest.Tools;

public class TestProcess : IDisposable
{
    public Process Process { get; }
    public List<string> Output { get; private set; }
    public List<string> Errors { get; private set; }
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

        Process = CreateProcess(executablePath);
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
        startInfo.Arguments =  ConfigPath == "" ? ConfigPath : @$"""{ConfigPath}""";;
        startInfo.FileName = executablePath;

        return startInfo;
    }

    public void Start()
    {
        Process.ErrorDataReceived += (sender, args) => Errors.Add(args.Data);
        Process.Start();
        Process.BeginErrorReadLine();
        processStarted = true;
    }

    public void GiveInput(string input)
    {
        Output = new List<string>();
        Errors = new List<string>();
        Inputs.Add(input);
        Process.StandardInput.WriteLine(input);
        AwaitProcessResponse();
    }

    public void Nop()
    {
        AwaitPrompt();
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
        return GetAllOutput().Where(s => !string.IsNullOrWhiteSpace(s));
    }

    public List<string> GetLastOutput()
    {
        return Output;
    }

    public string GetError()
    {
        return string.Join("\n", Errors);
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
        string output = ReadStandardOutputToString();

        Output = output.Split(Environment.NewLine).ToList();

        AllOutputs.Add(Output);
        AllErrors.Add(Errors);
    }

    private string ReadStandardOutputToString()
    {
        List<char> chars = new List<char> ();
        bool encounteredPrompt = false;
        while (!Process.StandardOutput.EndOfStream)
        {
            if ((char)Process.StandardOutput.Peek() == '$')
            {
                if (encounteredPrompt)
                {
                    break;
                }
                encounteredPrompt = true;
            }
            char chr = (char)Process.StandardOutput.Read();
            chars.Add(chr);
        }

        return new String(chars.ToArray());
    }

    private void AwaitPrompt()
    {
        while (!Process.StandardOutput.EndOfStream)
        {
            if ((char)Process.StandardOutput.Peek() == '$')
            {
                return;
            }

            Process.StandardOutput.Read();
        }
    }

    public void Dispose()
    {
        if (processStarted)
        {
            Process.Kill();
            Process.Dispose();
        }
    }
}