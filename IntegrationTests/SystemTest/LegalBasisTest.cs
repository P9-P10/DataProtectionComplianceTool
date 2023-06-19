using FluentAssertions;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class LegalBasisTest : TestResources
{
    [Fact]
    public void LegalBasisCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddLegalBasis(process, TestLegalBasis);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();

        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<string, LegalBasis>(TestLegalBasis.Key!,
                SystemOperation.Operation.Created, null));
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestLegalBasis.Key!,
                SystemOperation.Operation.Updated, TestLegalBasis));
    }

    [Fact]
    public void LegalBasisCanBeShown()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddLegalBasis(process, TestLegalBasis);
        ShowLegalBasis(process, TestLegalBasis);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestLegalBasis.ToListing()));
    }

    [Fact]
    public void LegalBasisCanBeUpdated()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddLegalBasis(process, TestLegalBasis);
        UpdateLegalBasis(process, TestLegalBasis, UpdatedTestLegalBasis);
        // ShowLegalBasis(process, UpdatedTestLegalBasis);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage(TestLegalBasis.Key!,
                SystemOperation.Operation.Updated, UpdatedTestLegalBasis));
        // output.Should().ContainSingle(s => s.Contains(UpdatedTestLegalBasis.ToListing()));
    }

    [Fact]
    public void LegalBasisCanBeListed()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddLegalBasis(process, TestLegalBasis);
        AddLegalBasis(process, UpdatedTestLegalBasis);
        ListLegalBasis(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestLegalBasis.ToListing()));
        output.Should().ContainSingle(s => s.Contains(UpdatedTestLegalBasis.ToListing()));
    }

    [Fact]
    public void LegalBasisCanBeDeleted()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddLegalBasis(process, TestLegalBasis);
        DeleteLegalBasis(process, TestLegalBasis);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetLastOutputNoWhitespaceOrPrompt().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s == FeedbackEmitterMessage.SuccessMessage<string, LegalBasis>(TestLegalBasis.Key!,
                SystemOperation.Operation.Deleted, null));
    }
}