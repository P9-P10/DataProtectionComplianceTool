﻿using FluentAssertions;
using Xunit;

namespace Test.SystemTest;

[Collection("SystemTestSequential")]
public class CommandlineTest
{
    [Fact]
    public void TestHelpCommand()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        
        process.GiveInput("help");
        string result = string.Join("", process.GetLastOutput());
        string error = process.GetError();
        result.Should().Be(@$"Using config found at {SystemTest.ConfigPath}" +
                           "$: Description:  This is a description of the root command" +
                           "Usage:  ! [command] [options]" +
                           "Options:  ?, h, help  Show help and usage information" +
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
    
}