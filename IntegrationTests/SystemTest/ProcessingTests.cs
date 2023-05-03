using FluentAssertions;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

[Collection("SystemTestSequential")]
public class ProcessingTests : TestResources
{
    [Fact]
    public void TestAdd_Prints_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        var firstResult = process.GetLastOutput();
        AddProcessing(process, TestProcessing);

        List<string> result = process.GetLastOutput();
        result[1].Should().Contain("ProcessingDescription");
        result[1].Should().Contain("ProcessingName");
        result[1].Should().Contain("Successfully");
    }

    [Fact]
    public void TestAdd_Processing_Stored_Correctly()
    {
    }

    [Fact]
    public void TestUpdate_Prints_Correct_Message()
    {
    }

    [Fact]
    public void TestUpdate_Processing_Stored_Correctly()
    {
    }

    [Fact]
    public void TestDelete_Prints_Correct_Message()
    {
    }

    [Fact]
    public void TestDelete_Removes_Processing_From_System()
    {
    }

    [Fact]
    public void TestShow_Returns_Correct_Value()
    {
    }
}