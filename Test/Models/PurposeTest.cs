using System.Collections.Generic;
using GraphManipulation.Models;
using Xunit;

namespace Test.Models;

public class PurposeTest
{
    [Fact]
    public void TestToListingWorksWithNullValues()
    {
        Purpose purpose = new Purpose()
        {
            Key = "Name",
            StorageRules = new List<StorageRule>()
        };

        Assert.Equal("Name, , False, [  ], [  ]", purpose.ToListing());
    }
    
    [Fact]
    public void TestToListingWorks()
    {
        Purpose purpose = new()
        {
            Key = "Name",
            StorageRules = new List<StorageRule>()
        };

        Assert.Equal("Name, , False, [  ], [  ]", purpose.ToListing());
    }
}