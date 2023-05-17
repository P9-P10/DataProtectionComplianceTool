using FluentAssertions;
using GraphManipulation.Commands;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;
using J2N;

namespace IntegrationTests.SystemTest;

public class StatusCommandTest : TestResources
{
    private static IEnumerable<string> FormatOutputForStatusTest(IEnumerable<string> list)
    {
        return list
            .Select(s => s.Replace(CommandLineInterface.Prompt, "").Trim())
            .Where(s => s.Contains("missing"));
    }

    private const string StringKey = "StringKey";
    private const int IntegerKey = 1;
    private static TableColumnPair TableColumnPair => new("Table", "Column");
    private static string TableColumnPairKey => $"{TableColumnPair.TableName} {TableColumnPair.ColumnName}";


    private static void CreateEntity(TestProcess testProcess, string commandName, string keyOption = OptionNamer.Name,
        string key = StringKey)
    {
        testProcess.GiveInput($"{commandName} {CommandNamer.Create} {keyOption} {key}");
    }

    [Fact]
    public void NothingToReportEmptyOutput()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutput());

        error.Should().BeEmpty();
        output.Should().BeEmpty();
    }

    [Fact]
    public void CreatingStorageRule()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.StorageRulesName);

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Count.Should().Be(2);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, StorageRule>(StringKey, "vacuuming condition"));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, StorageRule, PersonalDataColumn>(StringKey));
    }

    [Fact]
    public void CreatingPurpose()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.PurposesName);

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Count.Should().Be(3);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, Purpose>(StringKey, "legally required value"));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, Purpose, StorageRule>(StringKey));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, Purpose, VacuumingRule>(StringKey));
    }

    [Fact]
    public void CreatingVacuumingRule()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.VacuumingRulesName);

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Count.Should().Be(1);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, VacuumingRule>(StringKey, "interval"));
    }

    [Fact]
    public void CreatingProcessing()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.ProcessingsName);

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Count.Should().Be(2);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, Processing, Purpose>(StringKey));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<string, Processing, PersonalDataColumn>(StringKey));
    }

    [Fact]
    public void CreatingPersonalDataOrigin()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.PersonalDataOriginsName, OptionNamer.Id, IntegerKey.ToString());

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Count.Should().Be(3);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<int, PersonalDataOrigin, Individual>(IntegerKey));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<int, PersonalDataOrigin, PersonalDataColumn>(IntegerKey));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<int, PersonalDataOrigin, Origin>(IntegerKey));
    }

    [Fact]
    public void CreatingPersonalDataColumn()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.PersonalDataColumnsName, OptionNamer.TableColumn, TableColumnPairKey);

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Count.Should().Be(2);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<TableColumnPair, PersonalDataColumn, Purpose>(TableColumnPair));
        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<TableColumnPair, PersonalDataColumn>(TableColumnPair, "default value"));
    }

    [Fact]
    public void CreatingOrigin()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.OriginsName);

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Should().BeEmpty();
    }

    [Fact]
    public void CreatingIndividual()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();

        var outputBeforeReport = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        process.Start();

        CreateEntity(process, CommandNamer.IndividualsName, OptionNamer.Id, IntegerKey.ToString());
        ReportStatus(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        outputBeforeReport.SequenceEqual(output).Should().BeTrue();
        output.Should().BeEmpty();
    }

    [Fact]
    public void CreatingIndividualAfterPersonalDataColumn()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        CreateEntity(process, CommandNamer.PersonalDataColumnsName, OptionNamer.TableColumn, TableColumnPairKey);
        CreateEntity(process, CommandNamer.IndividualsName, OptionNamer.Id, IntegerKey.ToString());

        var error = process.GetAllErrorsNoWhitespace();
        var output = FormatOutputForStatusTest(process.GetLastOutputNoWhitespace()).ToList();

        error.Should().BeEmpty();
        output.Count.Should().Be(1);

        output.Should().ContainSingle(s => s == FeedbackEmitterMessage
            .MissingMessage<int, Individual>(IntegerKey, $"origin for '{TableColumnPair}'"));
    }
}