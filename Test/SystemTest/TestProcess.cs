using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Test.SystemTest;

public class TestProcess : IDisposable
{
    public Process Process { get; }
    public List<string> Output { get; private set; }
    public List<string> Errors { get; private set; }
    
    public List<List<string>> AllOutputs { get; }
    public List<List<string>> AllErrors { get; }

    public TestProcess(string executablePath)
    {
        AllOutputs = new List<List<string>>();
        AllErrors = new List<List<string>>();
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
        Process.OutputDataReceived += (sender, args) => Output.Add(args.Data);
        Process.ErrorDataReceived += (sender, args) => Errors.Add(args.Data);
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
    }

    public void GiveInput(string input)
    {
        Output = new List<string>();
        Errors = new List<string>();
        Process.StandardInput.WriteLine(input);
        Thread.Sleep(250);
    }
    
    public string GetOutput()
    {
        AwaitProcessResponse();
        return string.Join("", Output);
    }

    public List<string> GetAllOutput()
    {
        AwaitProcessResponse();
        return FlattenList(AllOutputs);
    }

    public List<string> GetLastOutput()
    {
        AwaitProcessResponse();
        return Output;
    }

    public string GetError()
    {
        AwaitProcessResponse();
        return string.Join("\n", Errors);
    }

    public List<string> GetAllErrors()
    {
        AwaitProcessResponse();
        return FlattenList(AllOutputs);
    }

    public List<string> GetLastError()
    {
        AwaitProcessResponse();
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
        Thread.Sleep(2000);
        AllOutputs.Add(Output);
        AllErrors.Add(Errors);
    }

    public void Dispose()
    {
        Process.Kill();
        Process.Dispose();
    }
}