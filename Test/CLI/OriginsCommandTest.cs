using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class OriginsCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IOriginsManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IOriginsManager>();
        
        return OriginsCommandBuilder.Build(console, managerMock.Object);
    }

    private const string Name = "originName";
    private const string Description = "This is a description";

    private static readonly IOrigin Origin = new Origin()
    {
        Name = Name,
        Description = Description,
        PersonalDataColumns = new List<PersonalDataColumn>()
    };

    public class Add
    {
        private const string CommandName = "add";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {Name} " +
                $"--description \"{Description}\"");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {Name} " +
                        $"--description \"{Description}\"");
            
            managerMock.Verify(manager => manager.Add(
                It.Is<string>(s => s == Name),
                It.Is<string>(s => s == Description)));
        }
    }
    
    public class Update
    {
        private const string CommandName = "update";
        
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
        }
    }
    
    public class Delete
    {
        private const string CommandName = "delete";
        
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
        }
    }
    
    public class Show
    {
        private const string CommandName = "show";
        
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
        }
    }
}