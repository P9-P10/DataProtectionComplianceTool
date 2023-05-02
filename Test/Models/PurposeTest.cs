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
            Columns = null
        };

        Assert.Equal("Name, , False, [  ], [  ], [  ]", purpose.ToListing());
    }
    
    [Fact]
    public void TestToListingWorks()
    {
        Purpose purpose = new Purpose()
        {
            Name = "Name",
            Columns = new List<PersonalDataColumn>()
        };

        Assert.Equal("Name, , False, [  ], [  ], [  ]", purpose.ToListing());
    }
}