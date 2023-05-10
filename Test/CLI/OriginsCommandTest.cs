using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using Moq;
using Xunit;

namespace Test.CLI;

public class OriginsCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IOriginsManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IOriginsManager>();

        managerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == Name)))
            .Returns(Origin);
        
        return new OriginsCommandBuilder(console, managerMock.Object).Build();
    }

    private const string Name = "originName";
    private const string NewName = "newOriginName";
    private const string Description = "This is a description";
    private const string NewDescription = "This is a new description";

    private static readonly Origin Origin = new()
    {
        Name = Name,
        Description = Description,
        PersonalDataColumns = new List<PersonalDataColumn>()
    };

    private static readonly Origin NewOrigin = new()
    {
        Name = NewName,
        Description = NewDescription
    };

    public class Add
    {
        private const string CommandName = "add";
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {NewName} " +
                $"--description \"{Description}\"");
        }
        
        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"-n {NewName} " +
                $"-d \"{Description}\"");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {NewName} " +
                        $"--description \"{NewDescription}\"");
            
            managerMock.Verify(manager => manager.Create(It.Is<string>(s => s == NewName)));
            managerMock.Verify(manager => manager.Update(
                It.Is<string>(s => s == NewName),
                It.Is<Origin>(o => o.Name == NewName && o.Description == NewDescription)));
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
                $"--new-name {NewName} " +
                $"--description \"{NewDescription}\" ");
        }
        
        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"-n {Name} " +
                $"-nn {NewName} " +
                $"-d \"{NewDescription}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {Name} " +
                        $"--new-name {NewName} " +
                        $"--description \"{NewDescription}\" ");
            
            managerMock.Verify(manager => manager.Update(
                It.Is<string>(s => s == NewName),
                It.Is<Origin>(o => o.Equals(NewOrigin))));
            
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
                $"--name {Name}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {Name}");
            
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
    }
}