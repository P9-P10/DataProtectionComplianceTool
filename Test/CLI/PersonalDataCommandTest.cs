using System;
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
    private static Command BuildCli(out Mock<IPersonalDataManager> personalDataManagerMock,
        out Mock<IPurposesManager> purposesManagerMock, out IConsole console)
    {
        return BuildCli(out personalDataManagerMock, out purposesManagerMock, out _, out _, out console);
    }

    private static Command BuildCli(out Mock<IPersonalDataManager> personalDataManagerMock,
        out Mock<IPurposesManager> purposesManagerMock, out Mock<IOriginsManager> originsManagerMock,
        out Mock<IIndividualsManager> individualsManagerMock, out IConsole console)
    {
        console = new TestConsole();
        personalDataManagerMock = new Mock<IPersonalDataManager>();

        personalDataManagerMock
            .Setup(manager =>
                manager.Get(It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1))))
            .Returns(new PersonalDataColumn
            {
                Description = Description,
                TableColumnPair = TableColumnPair1,
                Purposes = new List<Purpose>(),
                JoinCondition = JoinCondition
            });

        personalDataManagerMock
            .Setup(manager =>
                manager.Get(It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair2))))
            .Returns(new PersonalDataColumn
            {
                TableColumnPair = TableColumnPair2,
                Purposes = new List<Purpose> { new() { Name = Purpose1Name }, new() { Name = Purpose2Name } }
            });

        personalDataManagerMock
            .SetupSequence(manager =>
                manager.Get(It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair3))))
            .Returns(() => null)
            .Returns(new PersonalDataColumn
            {
                TableColumnPair = TableColumnPair3,
                Purposes = new List<Purpose>()
            });

        personalDataManagerMock
            .Setup(manager => manager.GetOriginOf(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1)),
                It.Is<int>(i => i == IndividualId)))
            .Returns(new Origin { Name = OriginName + "Other" });
        
        personalDataManagerMock
            .Setup(manager => manager.GetOriginOf(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair2)),
                It.Is<int>(i => i == IndividualId)))
            .Returns(Origin);

        purposesManagerMock = new Mock<IPurposesManager>();

        purposesManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == Purpose1Name)))
            .Returns(Purpose1);

        purposesManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == Purpose2Name)))
            .Returns(Purpose2);

        originsManagerMock = new Mock<IOriginsManager>();

        originsManagerMock
            .Setup(manager => manager.Get(It.Is<string>(s => s == OriginName)))
            .Returns(Origin);

        individualsManagerMock = new Mock<IIndividualsManager>();

        individualsManagerMock
            .Setup(manager => manager.Get(It.Is<int>(i => i == IndividualId)))
            .Returns(new Individual { Id = IndividualId });

        return PersonalDataCommandBuilder.Build(console, personalDataManagerMock.Object, purposesManagerMock.Object,
            originsManagerMock.Object, individualsManagerMock.Object);
    }

    private static readonly TableColumnPair TableColumnPair1 = new(TableName1, ColumnName1);
    private static readonly TableColumnPair TableColumnPair2 = new(TableName2, ColumnName2);
    private static readonly TableColumnPair TableColumnPair3 = new(TableName3, ColumnName3);
    private static readonly IPurpose Purpose1 = new Purpose { Name = Purpose1Name };
    private static readonly IPurpose Purpose2 = new Purpose { Name = Purpose2Name };

    private static readonly IOrigin Origin = new Origin
    {
        Name = OriginName,
        Description = "Origin description",
        PersonalDataColumns = new List<PersonalDataColumn>()
    };

    private const string TableName1 = "tableName";
    private const string ColumnName1 = "columnName";
    private const string TableName2 = "otherTable";
    private const string ColumnName2 = "otherColumn";
    private const string TableName3 = "yetAnotherTable";
    private const string ColumnName3 = "yetAnotherColumn";
    private const string JoinCondition = "tableName.id = columnName.id";
    private const string Description = "This is a description";
    private const string Purpose1Name = "purpose1";
    private const string Purpose2Name = "purpose2";
    
    private const int IndividualId = 12;
    private const string OriginName = "originName";

    public class Add
    {
        private const string CommandName = "add";

        [Fact]
        public void Parses()
        {
            VerifyCommand(
                BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName1} {ColumnName1} " +
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
                $"-tc {TableName1} {ColumnName1} " +
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
                $"--table-column {TableName1} {ColumnName1} " +
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
                $"--table-column {TableName1} {ColumnName1} " +
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
                $"--table-column {TableName1} {ColumnName1} " +
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
                $"--table-column {TableName1} {ColumnName1} " +
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
                        $"--table-column {TableName3} {ColumnName3} " +
                        $"--join-condition \"{JoinCondition}\" " +
                        $"--description \"{Description}\" " +
                        $"--purpose {Purpose1Name} " +
                        $"--purpose {Purpose2Name} "
                );

            managerMock.Verify(manager => manager.AddPersonalData(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair3)),
                It.Is<string>(s => s == JoinCondition),
                It.Is<string>(s => s == Description)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair3)),
                It.Is<string>(s => s == Purpose1Name)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair3)),
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
                $"--table-column {TableName1} {ColumnName1} " +
                $"--description \"{Description + "NEW"}\" "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName1} {ColumnName1} " +
                        $"--description \"{Description + "NEW"}\" "
                );
            managerMock.Verify(manager => manager.UpdateDescription(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1)),
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
                $"--table-column {TableName1} {ColumnName1} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName1} {ColumnName1} "
                );

            managerMock.Verify(manager => manager.Delete(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1))));
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
        private const string CommandName = "show";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName1} {ColumnName1} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName1} {ColumnName1} ");

            managerMock.Verify(manager => manager.Get(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1))));
        }

        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out _, out var console)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName1} {ColumnName1} ");

            console.Out.ToString().Should().StartWith($"({TableName1}, {ColumnName1}), {JoinCondition}, {Description}");
        }
    }

    public class AddPurpose
    {
        private const string CommandName = "add-purpose";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName1} {ColumnName1} " +
                $"--purpose {Purpose1Name} " +
                $"--purpose {Purpose2Name} "
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"-tc {TableName1} {ColumnName1} " +
                $"-p {Purpose1Name} " +
                $"-p {Purpose2Name} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName1} {ColumnName1} " +
                        $"--purpose {Purpose1Name} " +
                        $"--purpose {Purpose2Name} ");

            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1)),
                It.Is<string>(s => s == Purpose1Name)));
            managerMock.Verify(manager => manager.AddPurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1)),
                It.Is<string>(s => s == Purpose2Name)));
        }
    }

    public class RemovePurpose
    {
        private const string CommandName = "remove-purpose";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName1} {ColumnName1} " +
                $"--purpose {Purpose1Name} " +
                $"--purpose {Purpose2Name} "
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName2} {ColumnName2} " +
                        $"--purpose {Purpose1Name} " +
                        $"--purpose {Purpose2Name} "
                );

            managerMock.Verify(manager => manager.RemovePurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair2)),
                It.Is<string>(s => s == Purpose1Name)));
            managerMock.Verify(manager => manager.RemovePurpose(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair2)),
                It.Is<string>(s => s == Purpose2Name)));
        }
    }

    public class SetOrigin
    {
        private const string CommandName = "set-origin";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"--table-column {TableName1} {ColumnName1} " +
                $"--id {IndividualId} " +
                $"--origin {OriginName}"
            );
        }

        [Fact]
        public void AliasParses()
        {
            VerifyCommand(BuildCli(out _, out _, out _),
                $"{CommandName} " +
                $"-tc {TableName1} {ColumnName1} " +
                $"-i {IndividualId} " +
                $"-o {OriginName}"
            );
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName1} {ColumnName1} " +
                        $"--id {IndividualId} " +
                        $"--origin {OriginName}"
                );
            
            managerMock.Verify(manager => manager.SetOriginOf(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair1)),
                It.Is<int>(i => i == IndividualId),
                It.Is<string>(s => s == OriginName)));
        }
    }

    public class ShowOrigin
    {
        private const string CommandName = "show-origin";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _, out _), 
                $"{CommandName} " +
                $"--table-column {TableName2} {ColumnName2} " +
                $"--id {IndividualId} ");
        }

        [Fact]
        public void CallsManagerWithCorrectArguments()
        {
            BuildCli(out var managerMock, out _, out _)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName2} {ColumnName2} " +
                        $"--id {IndividualId} ");
            
            managerMock.Verify(manager => manager.GetOriginOf(
                It.Is<TableColumnPair>(pair => pair.Equals(TableColumnPair2)),
                It.Is<int>(i => i == IndividualId)));
        }
        
        [Fact]
        public void PrintsToConsole()
        {
            BuildCli(out _, out _, out var console)
                .Invoke($"{CommandName} " +
                        $"--table-column {TableName2} {ColumnName2} " +
                        $"--id {IndividualId} ");

            console.Out.ToString().Should().Be(Origin.ToListing() + Environment.NewLine);
        }
    }
}