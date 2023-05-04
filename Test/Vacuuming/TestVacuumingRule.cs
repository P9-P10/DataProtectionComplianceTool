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
            Name = "Name",
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
            Name = "Name",
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
            Name = "Name",
            Purposes = new List<Purpose>()
        };
        
        Assert.Equal("Name, , , [  ]", vacuumingRule.ToListing());
    }
}