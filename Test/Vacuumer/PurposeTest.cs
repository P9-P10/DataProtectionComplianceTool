using System;
using System.Globalization;
using GraphManipulation.Vacuuming.Components;
using Xunit;

namespace Test.Vacuumer;

public class PurposeTest
{
    [Fact]
    public void PurposeInitializationPopulatesExpirationDateCorrectly()
    {
        string expectedTime = DateTime.Now.AddYears(-2).ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        Purpose purpose = new("Name", "2y", "Condition", "Origin", true);
        string expirationDate = purpose.GetExpirationDate;

        Assert.Equal(expirationDate, expectedTime);
    }

    [Fact]
    public void PurposeInitializationPopulatesExpirationDateCorrectlyAllFormats()
    {
        string expectedTime = DateTime.Now.AddYears(-2).AddMonths(-2).AddDays(-2).AddHours(-2).AddMinutes(-2)
            .ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture);
        Purpose purpose = new("Name", "2y 2m 2d 2h 2M", "Condition", "Origin", true);
        string expirationDate = purpose.GetExpirationDate;

        Assert.Equal(expirationDate, expectedTime);
    }
}