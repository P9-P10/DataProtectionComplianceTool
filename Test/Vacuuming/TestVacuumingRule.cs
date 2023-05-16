using System;
using System.Collections.Generic;
using GraphManipulation.Models;
using Xunit;

namespace Test.Vacuuming;

public class TestVacuumingRule
{
    [Fact]
    public void TestShouldExecute_Returns_True_When_It_Should_Execute()
    {
        VacuumingRule vacuumingRule = new()
        {
            Key = "Name",
            Description = "Description",
            Id = 0,
            Interval = "2y",
            LastExecution = DateTime.Now.AddYears(-2).AddDays(-1)
        };

        Assert.True(vacuumingRule.ShouldExecute());
    }

    [Fact]
    public void TestShouldExecute_Returns_False_When_It_Should_Not_Execute()
    {
        VacuumingRule vacuumingRule = new()
        {
            Key = "Name",
            Description = "Description",
            Id = 0,
            Interval = "5y",
            LastExecution = DateTime.Now.AddYears(-2).AddDays(-1)
        };

        Assert.False(vacuumingRule.ShouldExecute());
    }

    [Fact]
    public void TestToListingWorksWithNullValues()
    {
        VacuumingRule vacuumingRule = new()
        {
            Key = "Name",
            Purposes = new List<Purpose>()
        };

        Assert.Equal("Name, None, None, None, Empty", vacuumingRule.ToListing());
    }

    [Fact]
    public void SetInterval_Updates_Interval_If_Given_Valid_Interval()
    {
        VacuumingRule vacuumingRule = new()
        {
            Key = "Name",
            Purposes = new List<Purpose>()
        };

        vacuumingRule.Interval = "2y 5d";
        Assert.Equal("2y 5d", vacuumingRule.Interval);
    }

    [Fact]
    public void SetInterval_Throws_Exception_On_Invalid_Interval()
    {
        VacuumingRule vacuumingRule = new()
        {
            Key = "Name",
            Purposes = new List<Purpose>()
        };

        Assert.Throws<VacuumingRule.IntervalParseException>(()=> vacuumingRule.Interval = "Test");
        
    }
}