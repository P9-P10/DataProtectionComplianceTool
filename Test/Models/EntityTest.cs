using GraphManipulation.Models;
using Xunit;

namespace Test.Models;

public class EntityTest
{
    [Fact]
    public void ToListingWorksWithNullValuesString()
    {
        var entity = new Entity<string>();
        
        Assert.Equal("No key, None", entity.ToListing());
    }
    
    [Fact]
    public void ToListingWorksWithNullValuesInt()
    {
        var entity = new Entity<int>();
        
        Assert.Equal("0, None", entity.ToListing());
    }
}