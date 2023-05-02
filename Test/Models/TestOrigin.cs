using GraphManipulation.Models;
using Xunit;

namespace Test;

public class TestOrigin
{
    [Fact]
    public void TestToListingWorksWithNullValues()
    {
        Origin origin = new Origin()
        {
            Name = "Name",
            PersonalDataColumns = null
        };
        
        Assert.Equal("Name, , [   ]",origin.ToListing());
    }
}