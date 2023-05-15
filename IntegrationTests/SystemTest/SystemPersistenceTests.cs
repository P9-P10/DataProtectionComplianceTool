using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class SystemPersistenceTests : TestResources
{
    [Fact]
    public void DeleteConditionsPersisted()
    {
        var process = Tools.SystemTest.CreateTestProcess();
        string configPath = process.ConfigPath;
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
        ListPurpose(process);
        List<string> firstResult = process.GetLastOutput();
        process.Dispose();

        TestProcess secondProcess = new(Tools.SystemTest.ExecutablePath, configPath);
        secondProcess.Start();
        secondProcess.AwaitReady();
        
        ListPurpose(secondProcess);

        
        List<string> secondResult = secondProcess.GetLastOutput();
        firstResult.FindAll(s => s.Contains(TestDeleteCondition.Key)
                                 && s.Contains(TestDeleteCondition.Description)).Should().ContainSingle();
        secondResult.FindAll(s => s.Contains(TestDeleteCondition.Key)
                                  && s.Contains(TestDeleteCondition.Description)).Should().ContainSingle();
        Assert.Equal(firstResult, secondResult);
        
        secondProcess.Dispose();
    }
}