using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using Moq;
using Xunit;

namespace Test.CLI;

public class VacuumingRulesCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IVacuumingRulesManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IVacuumingRulesManager>();

        managerMock
            .SetupSequence(manager => manager.Get(It.Is<string>(s => s == NewRuleName)))
            .Returns(() => null)
            .Returns(new VacuumingRule { Name = NewRuleName });

        managerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == RuleName)))
            .Returns(new VacuumingRule
            {
                Name = RuleName,
                Description = Description,
                Interval = Interval,
                Purposes = new List<Purpose>()
            });

        var purposesManagerMock = new Mock<IPurposesManager>();

        purposesManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == PurposeName)))
            .Returns(new Purpose { Name = PurposeName });

        return VacuumingRulesCommandBuilder.Build(console, managerMock.Object, purposesManagerMock.Object);
    }

    private const string RuleName = "ruleName";
    private const string NewRuleName = "newRuleName";
    private const string Interval = "This is an interval";
    private const string NewInterval = "This is a new interval";
    private const string Description = "This is a description";
    private const string NewDescription = "This is a new description";
    private const string PurposeName = "purposeName";
    private const string NewPurposeName = "newPurposeName";

    public class Add
    {
        private const string CommandName = "add";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _),
                $"{CommandName} " +
                $"--name {NewRuleName} " +
                $"--interval \"{Interval}\" " +
                $"--description \"{Description}\" " +
                $"--purpose {PurposeName} ");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {NewRuleName} " +
                        $"--interval \"{Interval}\" " +
                        $"--description \"{Description}\" " +
                        $"--purpose {PurposeName} ");
            
            managerMock.Verify(manager => manager.AddVacuumingRule(
                It.Is<string>(s => s == NewRuleName),
                It.Is<string>(s => s == Interval),
                It.Is<string>(s => s == PurposeName)));
            
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == NewRuleName),
                It.Is<string>(s => s == Description)));
        }
    }

    public class Update
    {
        private const string CommandName = "update";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {RuleName} " +
                $"--new-name {NewRuleName} " +
                $"--interval \"{NewInterval}\" " +
                $"--description \"{NewDescription}\" ");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {RuleName} " +
                        $"--new-name {NewRuleName} " +
                        $"--interval \"{NewInterval}\" " +
                        $"--description \"{NewDescription}\" ");
            
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == RuleName),
                It.Is<string>(s => s == NewDescription)));
            managerMock.Verify(manager => manager.UpdateInterval(
                It.Is<string>(s => s == RuleName),
                It.Is<string>(s => s == NewInterval)));
            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == RuleName),
                It.Is<string>(s => s == NewRuleName)));
        }
    }

    public class Delete
    {
        private const string CommandName = "delete";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {RuleName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {RuleName}");
            
            managerMock.Verify(manager => manager.Delete(It.Is<string>(s => s == RuleName)));
        }
    }

    public class List
    {
        private const string CommandName = "list";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName}");
            
            managerMock.Verify(manager => manager.GetAll());
        }
    }

    public class Show
    {
        private const string CommandName = "show";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {RuleName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {RuleName}");
            
            managerMock.Verify(manager => manager.Get(It.Is<string>(s => s == RuleName)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName} --name {RuleName}");

            console.Out.ToString().Should()
                .StartWith($"{RuleName}, {Description}, {Interval}");
        }
    }
}