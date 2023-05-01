using GraphManipulation.Models;
using Xunit;

namespace Test;

public class TestPurpose
{
    [Fact]
    public void TestToListinWorksWithNullValues()
    {
        Purpose purpose = new Purpose()
        {
            Name = "Name",
            Columns = null
        };

        Assert.Equal("Name, , False, [  ], [  ], [  ]", purpose.ToListing());
    }
}