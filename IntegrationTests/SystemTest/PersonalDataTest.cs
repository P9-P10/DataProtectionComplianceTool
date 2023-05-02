using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

[Collection("SystemTestSequential")]
public class PersonalDataTest : TestResources
{
    [Fact]
    public void AddIsSuccessful()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
    }
}