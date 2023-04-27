using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using FluentAssertions;
using GraphManipulation.Commands.Builders;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers.Interfaces;
using Moq;
using Xunit;

namespace Test.CLI;

public class LoggingCommandTest : CommandTest
{
    private static Command BuildCli(out Mock<ILogger> loggerMock, out IConsole console)
    {
        console = new TestConsole();
        loggerMock = new Mock<ILogger>();

        loggerMock
            .Setup(loggerMock => loggerMock.Read(It.IsAny<ILogConstraints>()))
            .Returns(new List<ILog>().OrderBy(l => l.LogNumber));

        return LoggingCommandBuilder.Build(console, loggerMock.Object);
    }

    public class List
    {
        private const string CommandName = "list";

        [Fact]
        public void Parses()
        {
            VerifyCommand(BuildCli(out _, out _), $"{CommandName}");
        }

        public class Numbers
        {
            [Fact]
            public void NumbersOptionParses()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} --numbers 3 --numbers 5");
            }

            [Fact]
            public void NumbersOptionWithAliasesParses()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -n 3 5");
            }

            [Fact]
            public void NumbersOptionOnlyOneFails()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -n 3", false);
            }

            [Fact]
            public void NumbersOptionMoreThanTwoFails()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -n 3 -n 5 -n 9", false);
            }

            [Fact]
            public void NumbersOptionMoreThanTwoCoalesceFails()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -n 3 5 9", false);
            }

            [Fact]
            public void NumbersOptionMinimumHigherThanMaximumFails()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -n 5 3", false);
            }
        }

        public class DateTimes
        {
            [Fact]
            public void DateTimesOptionParses()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} --date-times 2000/04/12T12:34:56 --date-times 3000/06/15T09:38:12");
            }

            [Fact]
            public void DateTimesOptionWithAliasesParses()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} -d 2000/04/12T12:34:56 3000/06/15T09:38:12");
            }

            [Fact]
            public void DateTimesOptionDifferentTimeFormatsParses()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -d 2000/04/12 3000/06/15T09:38");
            }

            [Fact]
            public void DateTimesOptionOnlyOneFails()
            {
                VerifyCommand(BuildCli(out _, out _), $"{CommandName} -d 2000/04/12T12:34:56", false);
            }

            [Fact]
            public void DateTimesOptionMoreThanTwoFails()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} -d 2000/04/12T12:34:56 -d 3000/06/15T09:38:12 -d 3000/06/15T09:38:13", false);
            }

            [Fact]
            public void DateTimesOptionMoreThanTwoCoalesceFails()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} -d 2000/04/12T12:34:56 3000/06/15T09:38:12 3000/06/15T09:38:13", false);
            }

            [Fact]
            public void DateTimesOptionMinimumHigherThanMaximumFails()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} -d 3000/06/15T09:38:12 2000/04/12T12:34:56", false);
            }
        }

        public class LogTypes
        {
            [Fact]
            public void LogTypesOptionParses()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} --log-types {LogType.Vacuuming} --log-types {LogType.Metadata}");
            }

            [Fact]
            public void LogTypesOptionWithAliasesParses()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} -lt {LogType.Vacuuming} {LogType.Metadata}");
            }
        }

        public class LogMessageFormats
        {
            [Fact]
            public void LogFormatsOptionParses()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} " +
                    $"--log-formats {LogMessageFormat.Plaintext} " +
                    $"--log-formats {LogMessageFormat.Json}");
            }

            [Fact]
            public void LogFormatsOptionWithAliasesParses()
            {
                VerifyCommand(BuildCli(out _, out _), 
                    $"{CommandName} -lf {LogMessageFormat.Plaintext} {LogMessageFormat.Json}");
            }
        }

        [Fact]
        public void AllOptionsParses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                "--numbers 3 --numbers 5 " +
                "--date-times 2000/04/12T12:34:56 --date-times 3000/06/15T09:38:12 " +
                $"--log-types {LogType.Vacuuming} --log-types {LogType.Metadata} " +
                $"--log-formats {LogMessageFormat.Plaintext} --log-formats {LogMessageFormat.Json}");
        }

        [Fact]
        public void AllOptionsWithAliasesParses()
        {
            VerifyCommand(BuildCli(out _, out _), 
                $"{CommandName} " +
                "-n 3 5 " +
                "-d 2000/04/12 3000/06/15T09:38 " +
                $"-lt {LogType.Vacuuming} {LogType.Metadata} " +
                $"-lf {LogMessageFormat.Plaintext} {LogMessageFormat.Json}");
        }

        [Fact]
        public void AllOptionsCallsLogger()
        {
            BuildCli(out var loggerMock, out _)
                .Invoke($"{CommandName} " +
                        "-n 3 5 " +
                        "-d 2000/04/12 3000/06/15T09:38 " +
                        $"-lt {LogType.Vacuuming} {LogType.Metadata} " +
                        $"-lf {LogMessageFormat.Plaintext} {LogMessageFormat.Json}");

            var expected = new LogConstraints(
                new NumberRange(3, 5),
                new TimeRange(new DateTime(2000, 4, 12), new DateTime(3000, 6, 15, 9, 38, 0)),
                new List<LogType> { LogType.Vacuuming, LogType.Metadata },
                new List<LogMessageFormat> { LogMessageFormat.Plaintext, LogMessageFormat.Json }
            );

            loggerMock.Verify(logger => logger.Read(It.IsAny<LogConstraints>()));
        }
    }
}