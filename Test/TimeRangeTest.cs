using System;
using GraphManipulation.Helpers;
using Xunit;

namespace Test;

public class TimeRangeTest
{
    [Fact]
    public void DateTimeWithinRangeWithinReturnsTrue()
    {
        var start = new DateTime(3);
        var end = new DateTime(7);

        var range = new TimeRange(start, end);

        var probe = new DateTime(5);

        Assert.True(range.DateTimeWithinRange(probe));
    }

    [Fact]
    public void DateTimeWithinRangeBelowReturnsFalse()
    {
        var start = new DateTime(3);
        var end = new DateTime(7);

        var range = new TimeRange(start, end);

        var probe = new DateTime(1);

        Assert.False(range.DateTimeWithinRange(probe));
    }

    [Fact]
    public void DateTimeWithinRangeAboveReturnsFalse()
    {
        var start = new DateTime(3);
        var end = new DateTime(7);

        var range = new TimeRange(start, end);

        var probe = new DateTime(10);

        Assert.False(range.DateTimeWithinRange(probe));
    }
}