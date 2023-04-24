using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Reflection;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class PersonalDataCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<IPersonalDataManager> personalDataManagerMock, out Mock<IPurposesManager> purposesManagerMock, out IConsole console)
    {
        console = new TestConsole();
        personalDataManagerMock = new Mock<IPersonalDataManager>();

        personalDataManagerMock
            .Setup(manager => 
                manager.Get(It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair))))
            .Returns(new PersonalDataColumn
                { 
                    Description = Description, 
                    TableColumnPair = TableColumnPair, 
                    Purposes = new List<Purpose>(),
                    JoinCondition = JoinCondition
                });

        purposesManagerMock = new Mock<IPurposesManager>();

        purposesManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == Purpose1Name)))
            .Returns(Purpose1);
        
        purposesManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == Purpose2Name)))
            .Returns(Purpose2);

        return PersonalDataCommandBuilder.Build(console, personalDataManagerMock.Object, purposesManagerMock.Object);
    }

    private static readonly TableColumnPair TableColumnPair = new(TableName, ColumnName);
    private static readonly IPurpose Purpose1 = new Purpose { Name = Purpose1Name };
    private static readonly IPurpose Purpose2 = new Purpose { Name = Purpose2Name };
    
    private const string TableName = "tableName";
    private const string ColumnName = "columnName";
    private const string JoinCondition = "tableName.id = columnName.id";
    private const string Description = "This is a description";
    private const string Purpose1Name = "purpose1";
    private const string Purpose2Name = "purpose2";

    public class Add
    {
        private const string CommandName = "add";

        [Fact]
        public void Parses()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName} {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--description \"{Description}\" " +
                $"--purpose {Purpose1Name} " +
                $"--purpose {Purpose2Name} "
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"-tc {TableName} {ColumnName} " +
                $"-jc \"{JoinCondition}\" " +
                $"-d \"{Description}\" " +
                $"-p {Purpose1Name} " +
                $"-p {Purpose2Name} "
            );
        }

        [Fact]
        public void WithOnePurposeParses()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName} {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--description \"{Description}\" " +
                $"--purpose {Purpose1Name} "
            );
        }

        [Fact]
        public void NoPurposeFails()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName} {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--description \"{Description}\" ",
                false
            );
        }
        
        [Fact]
        public void NoJoinConditionFails()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName} {ColumnName} " +
                $"--description \"{Description}\" " +
                $"--purpose {Purpose1Name} " +
                $"--purpose {Purpose2Name} ",
                false
            );
        }

        [Fact]
        public void WithoutDescriptionParses()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName} {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--purpose {Purpose1Name} " +
                $"--purpose {Purpose2Name} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName} {ColumnName} " +
                        $"--join-condition \"{JoinCondition}\" " +
                        $"--description \"{Description}\" " +
                        $"--purpose {Purpose1Name} " +
                        $"--purpose {Purpose2Name} "
                );

            managerMock.Verify(manager => manager.AddPersonalData(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair)),
                It.Is<string>(s => s == JoinCondition),
                It.Is<string>(s => s == Description)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair)),
                It.Is<string>(s => s == Purpose1Name)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair)),
                It.Is<string>(s => s == Purpose2Name)));
        }
    }

    public class Update
    {
        private const string CommandName = "update";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} " +
                $"--description \"{Description + "NEW"}\" "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table {TableName} " +
                        $"--column {ColumnName} " +
                        $"--description \"{Description + "NEW"}\" "
                );
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<TableColumnPair>(pair => pair.TableName == TableName && pair.ColumnName == ColumnName),
                It.Is<string>(s => s == Description + "NEW")));
        }
    }

    public class Delete
    {
        private const string CommandName = "delete";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table {TableName} " +
                        $"--column {ColumnName} "
                );
            
            managerMock.Verify(manager => manager.Delete(
                It.Is<TableColumnPair>(pair => pair.TableName == TableName && pair.ColumnName == ColumnName)));
        }
    }

    public class List
    {
        private const string CommandName = "list";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _), $"{CommandName}");
        }
    }

    public class Show
    {
        private const string CommandName = "show";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _), $"{CommandName} --table {TableName} --column {ColumnName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} --table {TableName} --column {ColumnName}");
            
            managerMock.Verify(manager => manager.Get(
                It.Is<TableColumnPair>(pair => pair.TableName == TableName && pair.ColumnName == ColumnName)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out _, out var console)
                .Invoke($"{CommandName} --table {TableName} --column {ColumnName}");
            
            console.Out.ToString().Should().StartWith($"{TableName}, {ColumnName}, {JoinCondition}, {Description}");
        }
    }

    public class AddPurpose
    {
        private const string CommandName = "add-purpose";

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
        }
    }

    public class RemovePurpose
    {
        private const string CommandName = "remove-purpose";

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
        }
    }

    public class SetOrigin
    {
        private const string CommandName = "set-origin";

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
        }
    }

    public class ShowOrigin
    {
        private const string CommandName = "show-origin";

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
        }
    }
}