using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Test.SystemTest;

public class TestProcess : IDisposable
{
    public Process Process { get; }
    public List<string> Output { get; private set; }
    public List<string> Errors { get; private set; }
    public List<string> Inputs { get; }
    
    public List<List<string>> AllOutputs { get; }
    public List<List<string>> AllErrors { get; }

    public TestProcess(string executablePath)
    {
        AllOutputs = new List<List<string>>();
        AllErrors = new List<List<string>>();
        Inputs = new List<string>();
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
        startInfo.Arguments = "";
        startInfo.FileName = executablePath;

        return startInfo;
    }

    public void Start()
    {
        Process.Start();
    }

    public void GiveInput(string input)
    {
        Output = new List<string>();
        Errors = new List<string>();
        Inputs.Add(input);
        Process.StandardInput.WriteLine(input);
        AwaitProcessResponse();
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
        // This is a suboptimal way of obtaining the output
        // But it is asynchronous, and there is no way to tell when it is finished writing
        
        List<char> chars = new List<char> ();
        bool encounteredPrompt = false;
        while (true)
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
        
        string res = new String(chars.ToArray());

        Output = res.Split(Environment.NewLine).ToList();
        
        AllOutputs.Add(Output);
        AllErrors.Add(Errors);
    }

    public void Dispose()
    {
        Process.Kill();
        Process.Dispose();
    }
}