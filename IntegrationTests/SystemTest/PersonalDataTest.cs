using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class PersonalDataTest : TestResources
{
    [Fact]
    public void PersonalDataCanBeAdded()
    {
        using var process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);
    }

    [Fact]
    public void PersonalDataCanBeShown()
    {
        
    }

    [Fact]
    public void PersonalDataCanBeUpdated()
    {
        
    }

    [Fact]
    public void PersonalDataCanBeListed()
    {
        
    }

    [Fact]
    public void PersonalDataCanBeDeleted()
    {
        
    }

    [Fact]
    public void PersonalDataCanReceivePurposes()
    {
        
    }

    [Fact]
    public void PersonalDataCanHavePurposesRemoved()
    {
        
    }

    [Fact]
    public void PersonalDataCanSetOriginForAnIndividual()
    {
        
    }

    [Fact]
    public void PersonalDataCanShowOriginForAnIndividual()
    {
        
    }
}