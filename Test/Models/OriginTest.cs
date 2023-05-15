using GraphManipulation.Models;
using Xunit;

namespace Test;

public class OriginTest
{
    [Fact]
    public void TestToListingWorksWithNullValues()
    {
        Origin origin = new()
        {
            Key = "Name",
            PersonalDataColumns = null
        };
        
        Assert.Equal("Name, , [  ]",origin.ToListing());
    }
}