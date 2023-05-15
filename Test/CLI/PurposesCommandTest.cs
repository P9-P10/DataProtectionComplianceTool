using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models;
using Moq;
using Xunit;

namespace Test.CLI;

public class PurposesCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IPurposesManager> purposeManagerMock, out Mock<IDeleteConditionsManager> deleteConditionsManagerMock, out IConsole console)
    {
        console = new TestConsole();
        purposeManagerMock = new Mock<IPurposesManager>();

        purposeManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == PurposeName)))
            .Returns(new Purpose
            {
                Name = PurposeName,
                Description = Description,
                LegallyRequired = LegallyRequired,
                Rules = new List<VacuumingRule>(),
                DeleteConditions = new List<DeleteCondition>(){DeleteCondition}
            });
        
        purposeManagerMock
            .SetupSequence(manager => manager.Get(It.Is<string>(s => s == PurposeWithoutDeleteConditionName)))
            .Returns(() => null)
            .Returns(new Purpose
            {
                Name = PurposeName,
                Description = Description,
                LegallyRequired = LegallyRequired,
                Rules = new List<VacuumingRule>()
            });

        deleteConditionsManagerMock = new Mock<IDeleteConditionsManager>();

        deleteConditionsManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == DeleteCondition.GetName())))
            .Returns(DeleteCondition);
        
        deleteConditionsManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == NewDeleteCondition.GetName())))
            .Returns(NewDeleteCondition);

        return PurposesCommandBuilder.Build(console, purposeManagerMock.Object, deleteConditionsManagerMock.Object);
    }

    private const string PurposeName = "purposeName";
    private const string PurposeWithoutDeleteConditionName = "noDeleteCondition";
    private const string Description = "This is a description";
    private const bool LegallyRequired = true;

    private static readonly DeleteCondition DeleteCondition = new()
    {
        Condition = "This is a condition",
        Name = "deleteCondition",
        Description = "This is a description"
    };

    private static readonly DeleteCondition NewDeleteCondition = new()
    {
        Condition = "This is a new condition",
        Name = "newDeleteCondition",
        Description = "This is a new description"
    };

    public class Create
    {
        private const string CommandName = CommandNamer.Create;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--name {PurposeName} " +
                $"--description \"{Description}\" " +
                $"--legally-required {LegallyRequired} " +
                $"--delete-condition-name {DeleteCondition.GetName()} "
            );
        }
        
        [Fact]
        public void BaseParses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--name {PurposeName} " +
                $"--delete-condition-name {DeleteCondition.GetName()} "
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"-n {PurposeName} " +
                $"-d \"{Description}\" " +
                $"-lr {LegallyRequired} " +
                $"-dcn {DeleteCondition.GetName()} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var purposeManagerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"-n {PurposeWithoutDeleteConditionName} " +
                        $"-d \"{Description}\" " +
                        $"-lr {LegallyRequired} " +
                        $"-dcn {DeleteCondition.GetName()} ");

            purposeManagerMock.Verify(manager => manager.Add(
                It.Is<string>(s => s == PurposeWithoutDeleteConditionName),
                It.Is<bool>(s => s == true),
                It.Is<string>(s => s == Description)));
            
            purposeManagerMock.Verify(manager => manager.SetDeleteCondition(
                It.Is<string>(s => s == PurposeWithoutDeleteConditionName),
                It.Is<string>(s => s == DeleteCondition.GetName())));
        }
        
        [Fact]
        public void BaseCallsManagerWithCorrectArguments()
        {
            BuildCli(out var purposeManagerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"-n {PurposeWithoutDeleteConditionName} " +
                        $"-dcn {DeleteCondition.GetName()} ");

            purposeManagerMock.Verify(manager => manager.Add(
                It.Is<string>(s => s == PurposeWithoutDeleteConditionName),
                It.Is<bool>(s => s == false),
                It.Is<string>(s => s == "")));
            
            purposeManagerMock.Verify(manager => manager.SetDeleteCondition(
                It.Is<string>(s => s == PurposeWithoutDeleteConditionName),
                It.Is<string>(s => s == DeleteCondition.GetName())));
        }
    }
    
    

    public class Update
    {
        private const string CommandName = CommandNamer.Update;

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--name {PurposeName} " +
                $"--new-name {PurposeName + "NEW"} " +
                $"--description \"{Description + "NEW"}\" " +
                $"--legally-required {!LegallyRequired} " +
                $"--delete-condition-name {NewDeleteCondition.GetName()} " 
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"-n {PurposeName} " +
                $"-nn {PurposeName + "NEW"} " +
                $"-d \"{Description + "NEW"}\" " +
                $"-lr {!LegallyRequired} " +
                $"-dcn {NewDeleteCondition.GetName()} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"-n {PurposeName} " +
                        $"-nn {PurposeName + "NEW"} " +
                        $"-d \"{Description + "NEW"}\" " +
                        $"-lr {!LegallyRequired} " +
                        $"-dcn {NewDeleteCondition.GetName()} ");

            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == PurposeName + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == Description + "NEW")));
            managerMock.Verify(manager => manager.UpdateLegallyRequired(
                It.Is<string>(s => s == PurposeName),
                It.Is<bool>(s => s == !LegallyRequired)));
            managerMock.Verify(manager => manager.SetDeleteCondition(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == NewDeleteCondition.GetName())));
        }

        [Fact]
        public void CallsManagerWithOnlyOneArgument()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--name {PurposeName} " +
                        $"--new-name {PurposeName + "NEW"} ");

            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == PurposeName + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<string>()), Times.Never);
            managerMock.Verify(manager => manager.UpdateLegallyRequired(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<bool>()), Times.Never);
            managerMock.Verify(manager => manager.SetDeleteCondition(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == NewDeleteCondition.GetName())), Times.Never);
        }

        [Fact]
        public void CallsManagerWithOnlyNecessaryArgument()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--name {PurposeName} " +
                        $"--new-name {PurposeName + "NEW"} " +
                        $"--description \"{Description}\" " +
                        $"--legally-required {LegallyRequired} " +
                        $"--delete-condition-name {DeleteCondition.GetName()}");

            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == PurposeName + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<string>()), Times.Never);
            managerMock.Verify(manager => manager.UpdateLegallyRequired(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<bool>()), Times.Never);
            managerMock.Verify(manager => manager.SetDeleteCondition(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<string>()), Times.Never);
        }
    }

    public class Delete
    {
        private const string CommandName = CommandNamer.Delete;

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _), $"{CommandName} --name {PurposeName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} --name {PurposeName}");

            managerMock.Verify(manager => manager.Delete(It.Is<string>(s => s == PurposeName)));
        }
    }

    public class List
    {
        private const string CommandName = CommandNamer.List;

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _), $"{CommandName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName}");

            managerMock.Verify(manager => manager.GetAll());
        }
    }

    public class Show
    {
        private const string CommandName = CommandNamer.Show;

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _), $"{CommandName} --name {PurposeName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} --name {PurposeName}");

            managerMock.Verify(manager => manager.Get(It.Is<string>(s => s == PurposeName)));
        }

        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out _, out var console)
                .Invoke($"{CommandName} --name {PurposeName}");

            console.Out.ToString().Should()
                .StartWith($"{PurposeName}, {Description}, {LegallyRequired}, [ {DeleteCondition.ToListingIdentifier()} ]");
        }
    }
}