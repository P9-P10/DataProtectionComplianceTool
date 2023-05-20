using System;
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

    [Fact]
    public void CanPreBufferReadLine()
    {
        const string testString = "This is a test string";
        var memoryStream = new MemoryStream();
        var streamWriter = new StreamWriter(memoryStream);
        streamWriter.WriteLine(testString);
        streamWriter.Flush();

        memoryStream.Position = 0;
        
        var streamReader = new StreamReader(memoryStream);
        Console.SetIn(streamReader);

        Console.ReadLine().Should().Be(testString);
    }

    [Fact]
    public void CanPreBufferFromString()
    {
        const string testString = "This is a test string";
        Console.SetIn(new StringReader(testString));
        
        Console.ReadLine().Should().Be(testString);
    }
    
    [Fact]
    public void CanPreBufferMultipleFromString()
    {
        const string testString1 = "This is a test string 1";
        const string testString2 = "This is a test string 2";
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, testString1, testString2)));
        
        Console.ReadLine().Should().Be(testString1);
        Console.ReadLine().Should().Be(testString2);
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

    [Fact]
    public void CanPreBufferFromCustomReader()
    {
        const string testString = "This is a test string";
        Console.SetIn(new TestReader(testString));

        Console.ReadLine().Should().Be(testString);
    }
    
    [Fact]
    public void CanPreBufferFromCustomReaderUnlimitedTimes()
    {
        const string testString = "This is a test string";
        Console.SetIn(new TestReader(testString));

        Console.ReadLine().Should().Be(testString);
        Console.ReadLine().Should().Be(testString);
        Console.ReadLine().Should().Be(testString);
    }
}