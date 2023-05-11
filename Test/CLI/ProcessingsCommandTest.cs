using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class ProcessingsCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IProcessingsManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IProcessingsManager>();

        managerMock
            .SetupSequence(manager => manager.Get(It.Is<string>(s => s == NewProcessingName)))
            .Returns(() => null)
            .Returns(new Processing { Name = NewProcessingName });

        managerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == ProcessingName)))
            .Returns(new Processing
            {
                Name = ProcessingName,
                Description = Description,
                Purpose = Purpose,
                PersonalDataColumn = PersonalDataColumn
            });

        var purposesManagerMock = new Mock<IPurposesManager>();

        purposesManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == PurposeName)))
            .Returns(Purpose);

        var personalDataManagerMock = new Mock<IPersonalDataManager>();

        personalDataManagerMock
            .Setup(manager => manager.Get(It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair))))
            .Returns(PersonalDataColumn);
        
        return ProcessingsCommandBuilder.Build(console, managerMock.Object, personalDataManagerMock.Object, purposesManagerMock.Object);
    }

    private const string ProcessingName = "processingName";
    private const string NewProcessingName = "newProcessingName";
    private const string TableName = "tableName";
    private const string ColumnName = "columnName";
    private const string NewTableName = "newTableName";
    private const string NewColumnName = "newColumnName";
    private static readonly TableColumnPair TableColumnPair = new(TableName, ColumnName);
    private static readonly TableColumnPair NewTableColumnPair = new(NewTableName, NewColumnName);
    private const string PurposeName = "purposeName";
    private const string Description = "This is a description";
    private const string NewDescription = "This is a new description";

    private static readonly Purpose Purpose = new()
    {
        Name = PurposeName,
        Description = "This is a description of a purpose",
        PersonalDataColumns = new List<PersonalDataColumn>(),
        LegallyRequired = true,
        Rules = new List<VacuumingRule>()
    };

    private static readonly PersonalDataColumn PersonalDataColumn = new()
    {
        TableColumnPair = TableColumnPair,
        Description = "This is a description of personal data",
        Purposes = new List<Purpose>(),
        JoinCondition = "This is a join condition"
    };

    public class Create
    {
        private const string CommandName = CommandNamer.Create;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                $"--name {NewProcessingName} " +
                $"--table-column {TableName} {ColumnName} " +
                $"--purpose {PurposeName} " +
                $"--description \"{Description}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {NewProcessingName} " +
                        $"--table-column {TableName} {ColumnName} " +
                        $"--purpose {PurposeName} " +
                        $"--description \"{Description}\" ");
            
            managerMock.Verify(manager => manager.AddProcessing(
                It.Is<string>(s => s == NewProcessingName),
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair)),
                It.Is<string>(s => s == PurposeName),
                It.Is<string>(s => s == Description)));
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
                $"--name {ProcessingName} " +
                $"--new-name {NewProcessingName} " +
                $"--description \"{NewDescription}\" ");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--name {ProcessingName} " +
                        $"--new-name {NewProcessingName} " +
                        $"--description \"{NewDescription}\" ");
            
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<string>(s => s == ProcessingName),
                It.Is<string>(s => s == NewDescription)));
            managerMock.Verify(manager => manager.UpdateName(
                It.Is<string>(s => s == ProcessingName),
                It.Is<string>(s => s == NewProcessingName)));
        }
    }
    
    public class Delete
    {
        private const string CommandName = CommandNamer.Delete;
        
        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {ProcessingName}");
        }
        
        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {ProcessingName}");
            
            managerMock.Verify(manager => manager.Delete(It.Is<string>(s => s == ProcessingName)));
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
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --name {ProcessingName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --name {ProcessingName}");
            
            managerMock.Verify(manager => manager.Get(It.Is<string>(s => s == ProcessingName)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out var console)
                .Invoke($"{CommandName} --name {ProcessingName}");

            console.Out.ToString().Should()
                .StartWith($"{ProcessingName}, {Description}, {Purpose.ToListingIdentifier()}");
        }
    }
}