using GraphManipulation.Vacuuming.Components;
using Xunit;

namespace Test.Vacuumer;

public class TableColumnPairTest
{
    [Fact]
    public void TestAddPurposeAddsNewPurpose()
    {
        TableColumnPair tp = new("Table", "Column");
        Purpose purpose = new("Name", "2y", "ExpirationCondition", "Local", true);
        tp.AddPurpose(purpose);

        Assert.Contains(purpose, tp.GetPurposes);
    }

    [Fact]
    public void TestAddPurposeOnlyAddsPurposeOnce()
    {
        TableColumnPair tp = new("Table", "Column");
        Purpose purpose = new("Name", "2y", "ExpirationCondition", "Local", true);
        tp.AddPurpose(purpose);
        tp.AddPurpose(purpose);

        Assert.True(tp.GetPurposes.Count == 1);
    }

    [Fact]
    public void GetPurposesWithLegalRequirementsReturnsPurpose()
    {
        Purpose legalPurpose = new("Name", "2y", "Condition", "Origin", true);
        TableColumnPair tp = new("Table", "Column");
        tp.AddPurpose(legalPurpose);
        Assert.Contains(legalPurpose, tp.GetPurposeWithLegalReason());
    }

    [Fact]
    public void GetPurposesWithLegalRequirementsReturnsNoPurposesIfNoneExists()
    {
        Purpose legalPurpose = new("Name", "2y", "Condition", "Origin", false);
        TableColumnPair tp = new("Table", "Column");
        tp.AddPurpose(legalPurpose);
        Assert.DoesNotContain(legalPurpose, tp.GetPurposeWithLegalReason());
        Assert.Empty(tp.GetPurposeWithLegalReason());
    }

    [Fact]
    public void GetPurposesWithLegalRequirementsReturnsMultiplePurposesIfMultipleExistsWithLegalRequirements()
    {
        Purpose legalPurpose = new("Name", "2y", "Condition", "Origin", true);
        Purpose legalPurpose1 = new("NewName", "2y", "Condition", "Origin", true);
        Purpose nonLegalPurpose = new("NewName", "2y", "Condition", "Origin", false);
        TableColumnPair tp = new("Table", "Column");
        tp.AddPurpose(legalPurpose);
        tp.AddPurpose(nonLegalPurpose);
        tp.AddPurpose(legalPurpose1);
        Assert.Contains(legalPurpose, tp.GetPurposeWithLegalReason());
        Assert.Contains(legalPurpose1, tp.GetPurposeWithLegalReason());
        Assert.DoesNotContain(nonLegalPurpose, tp.GetPurposeWithLegalReason());
    }

    [Fact]
    public void TestGetPurposeWithOldestExpirationDate()
    {
        Purpose legalPurpose = new("Name", "5y", "Condition", "Origin", true);
        Purpose legalPurpose1 = new("NewName", "2y", "Condition", "Origin", true);
        Purpose nonLegalPurpose = new("NewName", "6y", "Condition", "Origin", false);
        TableColumnPair tp = new("Table", "Column");
        tp.AddPurpose(legalPurpose);
        tp.AddPurpose(nonLegalPurpose);
        tp.AddPurpose(legalPurpose1);

        Assert.Equal(nonLegalPurpose, tp.GetPurposeWithOldestExpirationDate());
    }
}