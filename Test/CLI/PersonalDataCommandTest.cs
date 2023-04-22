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
    private static Command BuildCli(out Mock<IPersonalDataManager> managerMock, out IConsole console)
    {
        console = new TestConsole();
        managerMock = new Mock<IPersonalDataManager>();

        managerMock
            .Setup(manager =>
                manager.Get(
                    It.Is<TableColumnPair>(pair => pair.TableName == TableName && pair.ColumnName == ColumnName)))
            .Returns(new PersonalDataColumn
                { 
                    Description = Description, 
                    TableColumnPair = new TableColumnPair(TableName, ColumnName), 
                    Purposes = new List<Purpose>(),
                    JoinCondition = JoinCondition
                });

        return PersonalDataCommandBuilder.Build(console, managerMock.Object);
    }

    private const string TableName = "tableName";
    private const string ColumnName = "columnName";
    private const string JoinCondition = "tableName.id = columnName.id";
    private const string Description = "This is a description";
    private const string Purpose1 = "purpose1";
    private const string Purpose2 = "purpose2";

    public class Add
    {
        private const string CommandName = "add";

        [Fact]
        public void Parses()
        {
            VerifyCommand(
                BuildCli(out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--description \"{Description}\" " +
                $"--purpose {Purpose1} " +
                $"--purpose {Purpose2} "
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(
                BuildCli(out _, out _),
                $"{CommandName} " +
                $"-t {TableName} " +
                $"-c {ColumnName} " +
                $"-jc \"{JoinCondition}\" " +
                $"-d \"{Description}\" " +
                $"-p {Purpose1} " +
                $"-p {Purpose2} "
            );
        }

        [Fact]
        public void WithOnePurposeParses()
        {
            VerifyCommand(
                BuildCli(out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--description \"{Description}\" " +
                $"--purpose {Purpose1} "
            );
        }

        [Fact]
        public void NoPurposeFails()
        {
            VerifyCommand(
                BuildCli(out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--description \"{Description}\" ",
                false
            );
        }

        [Fact]
        public void WithoutDescriptionParses()
        {
            VerifyCommand(
                BuildCli(out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} " +
                $"--join-condition \"{JoinCondition}\" " +
                $"--purpose {Purpose1} " +
                $"--purpose {Purpose2} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} " +
                        $"--table {TableName} " +
                        $"--column {ColumnName} " +
                        $"--join-condition \"{JoinCondition}\" " +
                        $"--description \"{Description}\" " +
                        $"--purpose {Purpose1} " +
                        $"--purpose {Purpose2} "
                );

            managerMock.Verify(manager => manager.AddPersonalData(
                It.Is<TableColumnPair>(pair => pair.ColumnName == ColumnName && pair.TableName == TableName),
                It.Is<string>(s => s == JoinCondition),
                It.Is<string>(s => s == Description)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.ColumnName == ColumnName && pair.TableName == TableName),
                It.Is<string>(s => s == Purpose1)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.ColumnName == ColumnName && pair.TableName == TableName),
                It.Is<string>(s => s == Purpose2)));
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
                $"--table {TableName} " +
                $"--column {ColumnName} " +
                $"--description \"{Description + "NEW"}\" "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
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
            VerifyCommand(BuildCli(out _, out _),
                $"{CommandName} " +
                $"--table {TableName} " +
                $"--column {ColumnName} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
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
            VerifyCommand(BuildCli(out _, out _), $"{CommandName}");
        }
    }

    public class Show
    {
        private const string CommandName = "show";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName} --table {TableName} --column {ColumnName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName} --table {TableName} --column {ColumnName}");
            
            managerMock.Verify(manager => manager.Get(
                It.Is<TableColumnPair>(pair => pair.TableName == TableName && pair.ColumnName == ColumnName)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out var console)
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
            VerifyCommand(BuildCli(out _, out _), $"{CommandName}");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _)
                .Invoke($"{CommandName}");
        }
    }

    public class RemovePurpose
    {
        private const string CommandName = "remove-purpose";

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

    public class SetOrigin
    {
        private const string CommandName = "set-origin";

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

    public class ShowOrigin
    {
        private const string CommandName = "show-origin";

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