using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class PurposesCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IPurposesManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IPurposesManager>();

        managerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == PurposeName)))
            .Returns(new Purpose { Name = PurposeName, Description = Description, LegallyRequired = LegallyRequired });

        return PurposesCommandBuilder.Build(console, managerMock.Object);
    }

    private const string PurposeName = "purposeName";
    private const string Description = "This is a description";
    private const bool LegallyRequired = true;

    public class Add
    {
        private const string CommandName = "add";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _),
                $"{CommandName} " +
                $"--name {PurposeName} " +
                $"--description \"{Description}\" " +
                $"--legally-required {LegallyRequired}"
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _),
                $"{CommandName} " +
                $"-n {PurposeName} " +
                $"-d \"{Description}\" " +
                $"-lr {LegallyRequired}"
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"-n {PurposeName} " +
                        $"-d \"{Description}\" " +
                        $"-lr {LegallyRequired}");

            managerMock.Verify(manager => manager.Add(
                It.Is<string>(s => s == PurposeName),
                It.Is<bool>(s => s == true),
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
                $"--name {PurposeName} " +
                $"--new-name {PurposeName + "NEW"} " +
                $"--description \"{Description + "NEW"}\" " +
                $"--legally-required {!LegallyRequired} "
            );
        }
        
        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _),
                $"{CommandName} " +
                $"-n {PurposeName} " +
                $"-nn {PurposeName + "NEW"} " +
                $"-d \"{Description + "NEW"}\" " +
                $"-lr {!LegallyRequired} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {PurposeName} " +
                        $"--new-name {PurposeName + "NEW"} " +
                        $"--description \"{Description + "NEW"}\" " +
                        $"--legally-required {!LegallyRequired} ");
            
            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == PurposeName + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == Description + "NEW")));
            managerMock.Verify(manager => manager.UpdateLegallyRequired(
                It.Is<string>(s => s == PurposeName),
                It.Is<bool>(s => s == !LegallyRequired)));
        }
        
        [Fact]
        public void CallsManagerWithOnlyOneArgument()
        {
            BuildCli(out var managerMock, out _)
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
        }
        
        [Fact]
        public void CallsManagerWithOnlyNecessaryArgument()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {PurposeName} " +
                        $"--new-name {PurposeName + "NEW"} " +
                        $"--description \"{Description}\" " +
                        $"--legally-required {LegallyRequired} ");
            
            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == PurposeName + "NEW")));
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<string>()), Times.Never);
            managerMock.Verify(manager => manager.UpdateLegallyRequired(
                It.Is<string>(s => s == PurposeName),
                It.IsAny<bool>()), Times.Never);
        }
    }

    public class Delete
    {
        private const string CommandName = "delete";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {PurposeName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {PurposeName}");
            
            managerMock.Verify(manager => manager.Delete(It.Is<string>(s => s == PurposeName)));
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
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {PurposeName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName}");
        }
    }

    public class SetDeleteCondition
    {
        private const string CommandName = "set-delete-condition";

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

    public class ShowDeleteCondition
    {
        private const string CommandName = "show-delete-condition";

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