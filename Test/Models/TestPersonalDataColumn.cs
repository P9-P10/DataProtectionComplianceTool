using System.Collections.Generic;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using Xunit;

namespace Test.Models;

public class TestPersonalDataColumn
{
    [Fact]
    public void TestAddPurposeWorksWithPurposesBeingNull()
    {
        PersonalDataColumn personalDataColumn = new();
        Purpose purpose = new()
        {
            Name = "Name"
        };
        personalDataColumn.AddPurpose(purpose);
        
        Assert.NotNull(personalDataColumn.Purposes);
        if (personalDataColumn.Purposes != null) Assert.Contains(purpose, personalDataColumn.Purposes);
    }

    [Fact]
    public void TestToListingWorksWithNullValue()
    {
        PersonalDataColumn personalDataColumn = new()
        {
            Purposes = null,
            TableColumnPair = new TableColumnPair("Name","ColumnName")
        };
        
        Assert.Equal("(Name, ColumnName), , , , [  ]",personalDataColumn.ToListing());
    }

    [Fact]
    public void TestGetPurposes_Returns_Empty_List_When_Null()
    {
        PersonalDataColumn personalDataColumn = new()
        {
            Purposes = null
        };
        
        Assert.Empty(personalDataColumn.GetPurposes());
    }
    
    [Fact]
    public void TestGetPurposes_Returns_List_With_Elements()
    {
        Purpose purpose = new Purpose()
        {
            Name = "Name"
        };
        PersonalDataColumn personalDataColumn = new()
        {
            Purposes = new List<Purpose>(){purpose}
        };
        
        Assert.NotEmpty(personalDataColumn.GetPurposes());
        Assert.Contains(purpose, personalDataColumn.GetPurposes());
    }
}