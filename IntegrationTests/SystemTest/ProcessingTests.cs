using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class ProcessingTests : TestResources
{
    [Fact]
    public void TestAdd_Prints_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s =>
            s.Contains("ProcessingName") && s.Contains("successfully") && s.Contains("created"));
        result.Should()
            .ContainSingle(s => s.Contains("successfully") && s.Contains("updated") && s.Contains(TestProcessing.ToListing()));
    }

    [Fact]
    public void TestAdd_Processing_Stored_Correctly()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        ListProcessing(process);
        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains(TestProcessing.ToListing()));
    }

    [Fact]
    public void TestUpdate_Prints_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        UpdateProcessing(process, TestProcessing, NewTestProcessing);

        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s =>
            s.Contains("updated") && s.Contains(NewTestProcessing.Key) && s.Contains(NewTestProcessing.Description));
    }

    [Fact]
    public void TestUpdate_Processing_Stored_Correctly()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        UpdateProcessing(process, TestProcessing, NewTestProcessing);

        ListProcessing(process);
        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains(NewTestProcessing.ToListing()));
    }

    [Fact]
    public void TestDelete_Prints_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        DeleteProcessing(process, TestProcessing);

        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains("deleted") && s.Contains(TestProcessing.Key));
    }

    [Fact]
    public void TestDelete_Removes_Processing_From_System()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        DeleteProcessing(process, TestProcessing);

        ListProcessing(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s => s.Contains(TestProcessing.Key)
                            && s.Contains(TestProcessing.Description)).Should().BeEmpty();
    }

    [Fact]
    public void TestShow_Returns_Correct_Value()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddPurpose(process, TestPurpose);
        AddPersonalData(process, TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        ShowProcessing(process, TestProcessing);
        var result = process.GetLastOutputNoWhitespaceOrPrompt();
        result.Should().ContainSingle(s => s.Contains(TestProcessing.ToListing()));
    }
}