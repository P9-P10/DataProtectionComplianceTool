using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Test.SystemTest;

public class TestProcess : IDisposable
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
        WaitForOutput(ref producedOutput);

        return string.Join("", Output);
    }

    public string GetError()
    {
        WaitForOutput(ref producedError);

        return string.Join("\n", Errors);
    }

    private void WaitForOutput(ref int outputCount)
    {
        // This is a suboptimal way of obtaining the output
        // But it is asynchronous, and there is no way to tell when it is finished writing
        int timeout = 100;
        int duration = 0;
        while (outputCount < 20 && duration < timeout)
        {
            Thread.Sleep(10);
            duration++;
        }
    }

    public void Dispose()
    {
        Process.Kill();
        Process.Dispose();
    }
}