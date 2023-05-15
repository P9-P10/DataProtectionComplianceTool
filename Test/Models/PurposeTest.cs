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
            Name = "Name",
            DeleteConditions = new List<StorageRule>()
        };

        Assert.Equal("Name, , False, [  ], [  ]", purpose.ToListing());
    }
    
    [Fact]
    public void TestToListingWorks()
    {
        Purpose purpose = new()
        {
            Name = "Name",
            DeleteConditions = new List<StorageRule>()
        };

        Assert.Equal("Name, , False, [  ], [  ]", purpose.ToListing());
    }
}