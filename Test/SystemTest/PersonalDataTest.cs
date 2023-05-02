using Xunit;

namespace Test.SystemTest;

[Collection("SystemTestSequential")]
public class PersonalDataTest : TestResources
{
    [Fact]
    public void AddIsSuccessful()
    {
        using var process = SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
    }
}