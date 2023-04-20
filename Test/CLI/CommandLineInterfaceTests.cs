using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using GraphManipulation.Commands;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using J2N.Text;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Test.CLI;

public class CommandLineInterfaceTests
{
    private static IEnumerable<string> CommandNames()
    {
        var type = typeof(CommandNamer);
        var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public);
        return fieldInfos.Select(f => f.Name);
    }

    private static RootCommand BuildTestCli(
        out Mock<IIndividualsManager> individualsManagerMock,
        out Mock<IPersonalDataManager> personalDataManagerMock,
        out Mock<IPurposesManager> purposesManagerMock,
        out Mock<IOriginsManager> originsManagerMock,
        out Mock<IVacuumingRulesManager> vacuumingRulesManagerMock,
        out Mock<IDeleteConditionsManager> deleteConditionsManagerMock,
        out Mock<IProcessingsManager> processingsManagerMock,
        out Mock<ILogger> loggerMock,
        out Mock<IConfigManager> configManagerMock)
    {
        individualsManagerMock = new Mock<IIndividualsManager>();
        personalDataManagerMock = new Mock<IPersonalDataManager>();
        purposesManagerMock = new Mock<IPurposesManager>();
        originsManagerMock = new Mock<IOriginsManager>();
        vacuumingRulesManagerMock = new Mock<IVacuumingRulesManager>();
        deleteConditionsManagerMock = new Mock<IDeleteConditionsManager>();
        processingsManagerMock = new Mock<IProcessingsManager>();
        loggerMock = new Mock<ILogger>();
        configManagerMock = new Mock<IConfigManager>();

        // individualsManagerMock.Setup(e => e.SetIndividualsSource(It.IsAny<TableColumnPair>()));

        return CommandLineInterfaceBuilder.Build(
            individualsManagerMock.Object,
            personalDataManagerMock.Object,
            purposesManagerMock.Object,
            originsManagerMock.Object,
            vacuumingRulesManagerMock.Object,
            deleteConditionsManagerMock.Object,
            processingsManagerMock.Object,
            loggerMock.Object,
            configManagerMock.Object);
    }

    private static void VerifyPath(Command cli, string command, string description, Action[] verifications,
        int expectedValue)
    {
        cli.Invoke(command)
            .Should()
            .Be(expectedValue, $"\"{command}\" is a" + 
                               (expectedValue == 1 ? "n in" : " ") + 
                               "correct command" + 
                               (!string.IsNullOrEmpty(description) ? $" and is testing that \"{description}\"" : ""));

        foreach (var verification in verifications)
        {
            verification();
        }
    }

    
    [Fact]
    public void HappyPaths()
    {
        var cli = BuildTestCli(
            out var individualsManagerMock,
            out var personalDataManagerMock,
            out var purposesManagerMock,
            out var originsManagerMock,
            out var vacuumingRulesManagerMock,
            out var deleteConditionsManagerMock,
            out var processingsManagerMock,
            out var loggerMock,
            out var configManagerMock
        );

        cli.Subcommands.SelectMany(command =>
        {
            return (command.Name switch
            {
                CommandNamer.IndividualsName => new[]
                {
                    (
                        Command: "set-source --table tableName --column columnName",
                        Description: "",
                        Verifications: new[]
                        {
                            () =>
                            {
                                individualsManagerMock.Verify(manager =>
                                    manager.SetIndividualsSource(It.Is<TableColumnPair>(pair =>
                                        pair.TableName == "tableName" && pair.ColumnName == "columnName")));
                            },
                        }
                    ),
                    (
                        Command: "sts -t tableName -c columnName",
                        Description: "Alias passes",
                        Verifications: Array.Empty<Action>()
                    ),
                },
                CommandNamer.PersonalDataName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.PurposesName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.OriginsName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.VacuumingRulesName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.DeleteConditionName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.ProcessingsName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.LoggingName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                CommandNamer.ConfigurationName => Array.Empty<(string Command, string Description, Action[] Verifications)>(),
                _ => throw new Exception($"\"{command.Name}\" is not currently supported")
            }).Select(tuple =>
            {
                tuple.Command = $"{command.Name} {tuple.Command}";
                return tuple;
            });
        }).ToList().ForEach(tuple => { VerifyPath(cli, tuple.Command, tuple.Description, tuple.Verifications, 0); });
    }

    [Fact]
    public void UnhappyPaths()
    {
       
    }
}