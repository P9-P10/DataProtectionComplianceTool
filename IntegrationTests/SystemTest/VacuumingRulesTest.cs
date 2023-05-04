using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class VacuumingRulesTest : TestResources
{
    [Fact]
    public void VacuumingRulesCanBeAdded()
    {
        using var process = Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process, TestPurpose);

        AddVacuumingRule(process, TestVacuumingRule);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();
        
        error.Should().BeEmpty();
        output.Should().ContainSingle(s =>
            s.Contains($"Successfully added {TestVacuumingRule.ToListingIdentifier()} vacuuming rule") &&
            s.Contains($"{TestVacuumingRule.GetInterval()}") &&
            s.Contains($"{TestVacuumingRule.GetPurposes().First().ToListingIdentifier()}"));
        output.Should().ContainSingle(s =>
            s.Contains(
                $"Successfully updated {TestVacuumingRule.ToListingIdentifier()} vacuuming rule with {TestVacuumingRule.GetDescription()}"));
    }
    
    [Fact]
    public void VacuumingRulesCanBeShown()
    {
        
    }
    
    [Fact]
    public void VacuumingRulesCanBeUpdated()
    {
        
    }
    
    [Fact]
    public void VacuumingRulesCanBeListed()
    {
        
    }
    
    [Fact]
    public void VacuumingRulesCanBeDeleted()
    {
        
    }
    
    [Fact]
    public void VacuumingRulesCanReceivePurposes()
    {
        
    }
    
    [Fact]
    public void VacuumingRulesCanHavePurposesRemoved()
    {
        
    }
    [Fact]
    public void VacuumingRulesCanBeExecuted()
    {
        
    }
    
}