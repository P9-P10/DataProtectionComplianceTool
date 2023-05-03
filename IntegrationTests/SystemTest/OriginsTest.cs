using FluentAssertions;
using GraphManipulation.Models;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class OriginsTest : TestResources
{
    [Fact]
    public void TestAddOrigin_Returns_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        Origin origin = new() {Name = name, Description = description};
        AddOrigin(process, origin);
        string result = process.GetOutput();

        result.Should().Contain(description);
        result.Should().Contain(name);
        result.Should().Contain("Successfully");
    }

    [Fact]
    public void TestAddOrigin_Origin_Stored_Correctly()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        Origin origin = new() {Name = name, Description = description};
        AddOrigin(process, origin);
        ListOrigins(process);
        List<string> result = process.GetLastOutput();

        result.Should().Contain($"{name}, {description}, [  ]");
    }

    [Fact]
    public void TestUpdateOrigin_Returns_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        Origin origin = new() {Name = name, Description = description};
        AddOrigin(process, origin);


        UpdateOrigin(process, origin, new Origin() {Description = description, Name = "NewName"});
        List<string> result = process.GetLastOutput();

        result.First().Should().Contain("NewName");
        result.First().Should().Contain(name);
        result.First().Should().Contain("Successfully");
        result.First().Should().Contain("updated");
    }

    [Fact]
    public void TestDeleteOrigin_Returns_Correct_Message()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        Origin origin = new() {Name = name, Description = description};
        AddOrigin(process, origin);
        
        DeleteOrigin(process, origin);

        List<string> result = process.GetLastOutput();

        result.First().Should().Contain("OriginName");
        result.First().Should().Contain("deleted");
        result.First().Should().Contain(name);
    }

    [Fact]
    public void TestDeleteOrigin_Value_No_Longer_Stored()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        Origin origin = new() {Name = name, Description = description};
        AddOrigin(process, origin);
        
        DeleteOrigin(process, origin);
        
        ListOrigins(process);

        List<string> result = process.GetLastOutput();

        result.FindAll(s=>s.Contains(name)).Should().BeEmpty();

    }

    [Fact]
    public void TestShowOrigin_Returns_Correct_Values()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        const string description = "This is the description";
        const string name = "OriginName";
        Origin origin = new() {Name = name, Description = description};
        AddOrigin(process, origin);
        
        ShowOrigin(process,origin);
        List<string> result = process.GetLastOutput();
        
        result.First().Should().Contain($"{name}, {description}, [  ]");
    }
}