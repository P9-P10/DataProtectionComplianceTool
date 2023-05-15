using System.CommandLine;
using System.CommandLine.IO;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models;
using Moq;
using Xunit;

namespace Test.CLI;

public class DeleteConditionsCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<Manager<StorageRule>> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<Manager<StorageRule>>();

        managerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == DeleteConditionName)))
            .Returns(new StorageRule
            {
                VacuumingCondition = Condition,
                Description = Description,
                Name = DeleteConditionName
            });
        
        return DeleteConditionsCommandBuilder.Build(console, managerMock.Object);
    }

    private const string DeleteConditionName = "deleteConditionName";
    private const string NewDeleteConditionName = "newDeleteConditionName";
    private const string Description = "This is a description";
    private const string Condition = "This is a condition";

    public class Create
    {
        private const string CommandName = CommandNamer.Create;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {DeleteConditionName} " +
                $"--description \"{Description}\" " +
                $"--condition \"{Condition}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {NewDeleteConditionName} " +
                        $"--description \"{Description}\" " +
                        $"--condition \"{Condition}\" ");
            
            managerMock.Verify(manager => manager.Add(
                It.Is<string>(s => s == NewDeleteConditionName),
                It.Is<string>(s => s == Description),
                It.Is<string>(s => s == Condition)));
        }
    }
    
    public class Update
    {
        private const string CommandName = CommandNamer.Update;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {DeleteConditionName} " +
                $"--new-name {DeleteConditionName + "NEW"} " +
                $"--description \"{Description + "NEW"}\" " +
                $"--condition \"{Condition + "NEW"}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {DeleteConditionName} " +
                        $"--new-name {DeleteConditionName + "NEW"} " +
                        $"--description \"{Description + "NEW"}\" " +
                        $"--condition \"{Condition + "NEW"}\" ");
            
            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == DeleteConditionName),
                It.Is<string>(s => s == DeleteConditionName + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == DeleteConditionName),
                It.Is<string>(s => s == Description + "NEW")));
            managerMock.Verify(manager => manager.UpdateCondition(
                It.Is<string>(s => s == DeleteConditionName),
                It.Is<string>(s => s == Condition + "NEW")));
        }
    }
    
    public class Delete
    {
        private const string CommandName = CommandNamer.Delete;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {DeleteConditionName}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {DeleteConditionName}");
            
            managerMock.Verify(manager => manager.Delete(It.Is<string>(s => s == DeleteConditionName)));
        }
    }
    
    public class List
    {
        private const string CommandName = CommandNamer.List;
        
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
        private const string CommandName = CommandNamer.Show;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {DeleteConditionName}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {DeleteConditionName}");
            
            managerMock.Verify(manager => manager.Get(It.Is<string>(s => s == DeleteConditionName)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName} --name {DeleteConditionName}");

            console.Out.ToString().Should()
                .StartWith($"{DeleteConditionName}, {Description}, {Condition}");
        }
    }
}