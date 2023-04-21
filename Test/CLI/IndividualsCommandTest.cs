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
    private static Command BuildCli(out Mock<IIndividualsManager> individualsManager)
    {
        return BuildCli(out individualsManager, out _);
    }
    
    private static Command BuildCli(out Mock<IIndividualsManager> individualsManager, out IConsole console)
    {
        console = new TestConsole();
        individualsManager = new Mock<IIndividualsManager>();
        individualsManager
            .Setup(manager => manager.GetAll())
            .Returns(new List<IIndividual> { new Individual { Id = 47 } });
        
        return IndividualsCommandBuilder.Build(console, individualsManager.Object);
    }

    private static void VerifyCommand(Command cli, string command, bool happy = true)
    {
        VerifyCommand(cli, command, out _, happy);
    }

    private static void VerifyCommand(Command cli, string command, out TestConsole console, bool happy = true)
    {
        console = new TestConsole();
        
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
            VerifyCommand(BuildCli(out _), $"{CommandName} --table tableName --column columnName");
        }

        [Fact]
        public void MissingRequiredOptionTableFails()
        {
            VerifyCommand(BuildCli(out _), $"{CommandName} --column columnName", false);
        }

        [Fact]
        public void MissingRequiredOptionColumnFails()
        {
            VerifyCommand(BuildCli(out _), $"{CommandName} --table tableName", false);
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
            VerifyCommand(BuildCli(out _), $"{CommandName}");
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
            VerifyCommand(BuildCli(out _), $"{CommandName}", out var console);
            console.Out.ToString()!.Trim().Should().Be("47");
        }
    }

    public class ShowIndividual
    {
        private const string CommandName = "show";
        
        [Fact]
        public void Parses()
        {
            
        }
    }
}