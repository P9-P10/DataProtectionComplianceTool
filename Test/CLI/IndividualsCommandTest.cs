using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class IndividualsCommandTest : CommandTest
{
    private const int IndividualId = 47;

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
            .Setup(manager => manager.Get(It.IsAny<int>()))
            .Returns(() => null);
        individualsManager
            .Setup(manager => manager.Get(IndividualId))
            .Returns(new Individual {Id = IndividualId});
        individualsManager
            .Setup(manager => manager.GetAll())
            .Returns(new List<IIndividual> {new Individual {Id = IndividualId}, new Individual()});

        return IndividualsCommandBuilder.Build(console, individualsManager.Object);
    }


    public class SetSource
    {
        private const string CommandName = "set-source";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --table-column tableName columnName");
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(), $"{CommandName} -tc tableName columnName");
        }

        [Fact]
        public void MissingRequiredArgumentFails()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --table-column tableName", false);
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var individualsManagerMock)
                .Invoke($"{CommandName} -tc tableName columnName");

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
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var individualsManagerMock)
                .Invoke($"{CommandName}");

            individualsManagerMock.Verify(manager => manager.GetAll());
        }

        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName}");

            console.Out.ToString().Should().Be("Id" + Environment.NewLine +
                                               IndividualId + Environment.NewLine
                                               + "Unknown" + Environment.NewLine);
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
        public void MissingRequiredOptionFails()
        {
            VerifyCommand(BuildCli(), $"{CommandName}", false);
        }

        [Fact]
        public void MissingValueForOptionFails()
        {
            VerifyCommand(BuildCli(), $"{CommandName} --id", false);
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var individualsManagerMock)
                .Invoke($"{CommandName} --id {IndividualId}");

            individualsManagerMock.Verify(manager =>
                manager.Get(It.Is<int>(i => i == IndividualId)));
        }

        [Fact]
        public void DoesNotCallsManagerIfInvalidCommand()
        {
            BuildCli(out var individualsManagerMock)
                .Invoke($"{CommandName}");

            individualsManagerMock.Verify(manager =>
                manager.Get(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void PrintsToConsoleWhenFound()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName} --id {IndividualId}");

            console.Out.ToString().Should().Be($"{IndividualId}" + Environment.NewLine);
        }

        [Fact]
        public void PrintsToConsoleWhenNotFound()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName} --id {IndividualId + 1}");

            console.Out.ToString().Should().StartWith("Could not find");
        }
    }
}