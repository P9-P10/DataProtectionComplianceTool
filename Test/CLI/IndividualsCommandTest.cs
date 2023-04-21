using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using Moq;
using VDS.RDF;
using Xunit;

namespace Test.CLI;

public class IndividualsCommandTest
{
    // private class TestTestConsole : IConsole
    // {
    //     public TestTestConsole()
    //     public IStandardStreamWriter Out { get; }
    //     public bool IsOutputRedirected { get; }
    //     public IStandardStreamWriter Error { get; }
    //     public bool IsErrorRedirected { get; }
    //     public bool IsInputRedirected { get; }
    // }

    private static int IndividualId = 47;

    private static Command BuildCli()
    {
        return BuildCli(out _, out _);
    }
    
    private static Command BuildCli(out Mock<IIndividualsManager> individualsManager)
    {
        return BuildCli(out individualsManager, out _);
    }

    private static Command BuildCli(out Mock<IIndividualsManager> individualsManager, out IConsole console)
    {
        console = new TestConsole();
        individualsManager = new Mock<IIndividualsManager>();

        individualsManager
            .Setup(manager => manager.Get(IndividualId))
            .Returns(new Individual { Id = IndividualId });
        individualsManager
            .Setup(manager => manager.GetAll())
            .Returns(new List<IIndividual> { new Individual { Id = IndividualId }, new Individual() });
        
        return IndividualsCommandBuilder.Build(console, individualsManager.Object);
    }

    private static void VerifyCommand(Command cli, string command, bool happy = true)
    {
        VerifyCommand(cli, command, new TestConsole(), happy);
    }

    private static void VerifyCommand(Command cli, string command, IConsole console, bool happy = true)
    {
        if (happy)
        {
            cli.Parse(command).Errors.Should().BeEmpty();
        }
        else
        {
            cli.Parse(command).Errors.Should().NotBeEmpty();
        }
        
        cli.Invoke(command, console).Should().Be(happy ? 0 : 1);
    }

    public class SetSource
    {
        private const string CommandName = "set-source";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --table tableName --column columnName");
        }

        [Fact]
        public void MissingRequiredOptionTableFails()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --column columnName", false);
        }

        [Fact]
        public void MissingRequiredOptionColumnFails()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --table tableName", false);
        }

        [Fact]
        public void CallsManager()
        {
            var cli = BuildCli(out var individualsManagerMock);
            VerifyCommand(cli, $"{CommandName} --table tableName --column columnName");
            individualsManagerMock.Verify(manager =>
                manager.SetIndividualsSource(It.Is<TableColumnPair>(pair => 
                    pair.TableName == "tableName" && pair.ColumnName == "columnName")));
        }
    }

    public class ListIndividuals
    {
        private const string CommandName = "list";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(), $"{CommandName}");
        }
        
        [Fact]
        public void CallsManager()
        {
            var cli = BuildCli(out var individualsManagerMock);
            VerifyCommand(cli, $"{CommandName}");
            individualsManagerMock.Verify(manager => manager.GetAll());
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            VerifyCommand(BuildCli(out _, out var console), $"{CommandName}");
            console.Out.ToString().Should().Be($"{IndividualId}\nUnknown\n");
        }
    }

    public class ShowIndividual
    {
        private const string CommandName = "show";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --id {IndividualId}");
        }
        
        [Fact]
        public void CallsManager()
        {
            var cli = BuildCli(out var individualsManagerMock);
            VerifyCommand(cli, $"{CommandName} --id {IndividualId}");
            individualsManagerMock.Verify(manager => 
                manager.Get(It.Is<int>(i => i == IndividualId)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            VerifyCommand(BuildCli(out _, out var console), $"{CommandName} --id {IndividualId}");
            console.Out.ToString().Should().Be($"{IndividualId}\n");
        }
    }
}