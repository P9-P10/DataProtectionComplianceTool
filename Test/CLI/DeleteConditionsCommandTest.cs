using System.CommandLine;
using System.CommandLine.IO;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using Moq;
using Xunit;

namespace Test.CLI;

public class DeleteConditionsCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IDeleteConditionsManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IDeleteConditionsManager>();

        managerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == Name)))
            .Returns(new DeleteCondition
            {
                Condition = Condition,
                Description = Description,
                Name = Name
            });
        
        return DeleteConditionsCommandBuilder.Build(console, managerMock.Object);
    }

    private const string Name = "deleteConditionName";
    private const string Description = "This is a description";
    private const string Condition = "This is a condition";

    public class Add
    {
        private const string CommandName = "add";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {Name} " +
                $"--description \"{Description}\" " +
                $"--condition \"{Condition}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {Name} " +
                        $"--description \"{Description}\" " +
                        $"--condition \"{Condition}\" ");
            
            managerMock.Verify(manager => manager.Add(
                It.Is<string>(s => s == Name),
                It.Is<string>(s => s == Description),
                It.Is<string>(s => s == Condition)));
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
                $"--name {Name} " +
                $"--new-name {Name + "NEW"} " +
                $"--description \"{Description + "NEW"}\" " +
                $"--condition \"{Condition + "NEW"}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {Name} " +
                        $"--new-name {Name + "NEW"} " +
                        $"--description \"{Description + "NEW"}\" " +
                        $"--condition \"{Condition + "NEW"}\" ");
            
            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == Name),
                It.Is<string>(s => s == Name + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == Name),
                It.Is<string>(s => s == Description + "NEW")));
            managerMock.Verify(manager => manager.UpdateCondition(
                It.Is<string>(s => s == Name),
                It.Is<string>(s => s == Condition + "NEW")));
        }
    }
    
    public class Delete
    {
        private const string CommandName = "delete";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {Name}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {Name}");
            
            managerMock.Verify(manager => manager.Delete(It.Is<string>(s => s == Name)));
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
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {Name}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {Name}");
            
            managerMock.Verify(manager => manager.Get(It.Is<string>(s => s == Name)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName} --name {Name}");

            console.Out.ToString().Should()
                .StartWith($"{Name}, {Description}, {Condition}");
        }
    }
}