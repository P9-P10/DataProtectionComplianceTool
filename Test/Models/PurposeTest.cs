using System.Collections.Generic;
using GraphManipulation.Models;
using Xunit;

namespace Test;

public class PurposeTest
{
    [Fact]
    public void TestToListingWorksWithNullValues()
    {
        Purpose purpose = new Purpose()
        {
            Name = "Name",
            PersonalDataColumns = null
        };

        Assert.Equal("Name, , False, [  ], [  ], [  ]", purpose.ToListing());
    }
    
    [Fact]
    public void TestToListingWorks()
    {
        Purpose purpose = new Purpose()
        {
            Name = "Name",
            PersonalDataColumns = new List<PersonalDataColumn>()
        };

        Assert.Equal("Name, , False, [  ], [  ], [  ]", purpose.ToListing());
    }
}