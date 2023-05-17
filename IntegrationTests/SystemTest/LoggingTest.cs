using FluentAssertions;
using GraphManipulation.Commands;
using GraphManipulation.Logging;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class LoggingTest : TestResources
{
    private static void CreateLogEntries(TestProcess testProcess, int number)
    {
        for (var i = 1; i <= number; i++)
        {
            AddLogEntryOrigin(testProcess, i.ToString());
        }
    }

    private static IEnumerable<string> FormatOutputForLogTest(IEnumerable<string> list)
    {
        return list.Select(s => s.Replace(CommandLineInterface.Prompt, ' ').Trim());
    }

    [Fact]
    public void OneLogEntryOneOutput()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const int numberOfEntries = 1;
        CreateLogEntries(process, numberOfEntries);

        var constraints = new LogConstraints(limit: numberOfEntries + 1);

        ListLogs(process, constraints);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForLogTest(process.GetAllOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        output.Last().Should().Be($"Showing all {numberOfEntries} log entries");

        var logEntries = output
            .TakeLast(numberOfEntries + 1)
            .Take(numberOfEntries)
            .Select(s => new Log(s))
            .ToList();

        logEntries.First().LogNumber.Should().Be(1);
        logEntries.Last().LogNumber.Should().Be(numberOfEntries);
    }

    [Fact]
    public void ManyEntriesManyOutputs()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const int numberOfEntries = 10;
        CreateLogEntries(process, numberOfEntries);

        var constraints = new LogConstraints(limit: numberOfEntries + 1);

        ListLogs(process, constraints);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForLogTest(process.GetAllOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        output.Last().Should().Be($"Showing all {numberOfEntries} log entries");

        var logEntries = output
            .TakeLast(numberOfEntries + 1)
            .Take(numberOfEntries)
            .Select(s => new Log(s))
            .ToList();

        logEntries.First().LogNumber.Should().Be(1);
        logEntries.Last().LogNumber.Should().Be(numberOfEntries);
    }

    [Fact]
    public void ManyEntriesLimitedOutputChangesMessage()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const int numberOfEntries = 10;
        CreateLogEntries(process, numberOfEntries);

        var constraints = new LogConstraints(limit: numberOfEntries);

        ListLogs(process, constraints);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForLogTest(process.GetAllOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        output.Last().Should().Be($"Showing newest {constraints.Limit} log entries");

        var logEntries = output
            .TakeLast(numberOfEntries + 1)
            .Take(numberOfEntries)
            .Select(s => new Log(s))
            .ToList();

        logEntries.First().LogNumber.Should().Be(1);
        logEntries.Last().LogNumber.Should().Be(numberOfEntries);
    }

    [Fact]
    public void ManyEntriesLimitedOutputChangesLogsShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const int numberOfEntries = 10;
        CreateLogEntries(process, numberOfEntries);

        var constraints = new LogConstraints(limit: numberOfEntries - 3);

        ListLogs(process, constraints);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForLogTest(process.GetAllOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        output.Last().Should().Be($"Showing newest {constraints.Limit} log entries");

        var logEntries = output
            .TakeLast(constraints.Limit + 1)
            .Take(constraints.Limit)
            .Select(s => new Log(s))
            .ToList();

        logEntries.First().LogNumber.Should().Be(numberOfEntries - constraints.Limit + 1);
        logEntries.Last().LogNumber.Should().Be(numberOfEntries);
    }

    public class Metadata
    {
    }

    public class System
    {
    }

    public class Vacuuming
    {
        [Fact]
        public void ExecutingVacuumingRuleResultsInLogEntry()
        {
            using var process = Tools.SystemTest.CreateTestProcess(out var dbConnection);
            process.Start();
            process.AwaitReady();

            SetupTestData(dbConnection);

            AddStorageRule(process, TestStorageRule);
            AddPurpose(process, TestPurpose);
            AddPersonalData(process, TestPersonalDataColumn);
            AddVacuumingRule(process, TestVacuumingRule);
            ExecuteVacuumingRule(process, new[] { TestVacuumingRule });

            var constraints = new LogConstraints(logTypes: new[] { LogType.Vacuuming });

            ListLogs(process, constraints);

            var error = process.GetAllErrorsNoWhitespace();
            var output = FormatOutputForLogTest(process.GetAllOutputNoWhitespace());

            error.Should().BeEmpty();
            var logEntry = output
                .TakeLast(2)
                .Take(1)
                .Select(s => new Log(s))
                .First();

            logEntry.Subject.Should().Be(TestPersonalDataColumn.ToListingIdentifier());
            logEntry.LogType.Should().Be(LogType.Vacuuming);
            logEntry.Message.Should().Contain(
                $"\"UPDATE {TestPersonalDataColumn.Key.TableName} " +
                $"SET {TestPersonalDataColumn.Key.ColumnName} = \'{TestPersonalDataColumn.DefaultValue}\' " +
                $"WHERE ({TestStorageRule.VacuumingCondition});\" " +
                $"affected {TestPersonalDataColumn.ToListingIdentifier()} " +
                $"because it is stored under the following purpose(s): {TestPurpose.ToListingIdentifier()}");
        }
    }
}