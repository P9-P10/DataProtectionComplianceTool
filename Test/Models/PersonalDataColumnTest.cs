using System.Collections.Generic;
using System.Linq;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using Xunit;

namespace Test.Models;

public class PersonalDataColumnTest
{
    [Fact]
    public void TestAddPurposeWorksWithPurposesBeingNull()
    {
        PersonalDataColumn personalDataColumn = new();
        Purpose purpose = new()
        {
            Key = "Name"
        };
        personalDataColumn.Purposes = personalDataColumn.Purposes.Append(purpose);
        
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
        
        Assert.Equal("(Name, ColumnName), , , [  ]",personalDataColumn.ToListing());
    }

    [Fact]
    public void TestGetPurposes_Returns_Empty_List_When_Null()
    {
        PersonalDataColumn personalDataColumn = new()
        {
            Purposes = null
        };
        
        Assert.Empty(personalDataColumn.Purposes);
    }
    
    [Fact]
    public void TestGetPurposes_Returns_List_With_Elements()
    {
        Purpose purpose = new Purpose()
        {
            Key = "Name"
        };
        PersonalDataColumn personalDataColumn = new()
        {
            Purposes = new List<Purpose>(){purpose}
        };
        
        Assert.NotEmpty(personalDataColumn.Purposes);
        Assert.Contains(purpose, personalDataColumn.Purposes);
    }
}