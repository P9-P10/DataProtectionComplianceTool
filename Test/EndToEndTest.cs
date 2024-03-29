﻿using System;
using System.IO;
using FluentAssertions;
using GraphManipulation;
using Xunit;

namespace Test;

public class EndToEndTest
{
    private MemoryStream OutputStream;

    private MemoryStream ErrorStream;
    
    public EndToEndTest()
    {
        // Create memory stream and redirect console output
        OutputStream = new MemoryStream();
        StreamWriter outputWriter = new StreamWriter(OutputStream);
        outputWriter.AutoFlush = true;
        Console.SetOut(outputWriter);

        // Do the same for standard error
        ErrorStream = new MemoryStream();
        StreamWriter errorWriter = new StreamWriter(ErrorStream);
        errorWriter.AutoFlush = true;
        Console.SetError(errorWriter);
    }

    private string ReadOutput()
    {
        return ReadStream(OutputStream);
    }

    private string ReadError()
    {
        return ReadStream(ErrorStream);
    }

    private string ReadStream(Stream stream)
    {
        StreamReader reader = new StreamReader(stream);
        stream.Position = 0;
        return reader.ReadToEnd();

    }

    [Fact]
    public void CanRunProgram()
    {
        TestProgram testProgram = new TestProgram();
        testProgram.Start();

        string output = ReadOutput();
        output.Should().Contain(testProgram.configPath);
    }
}