using System;
using System.Collections.Generic;
using GraphManipulation.Models;
using Xunit;

namespace Test.Vacuuming;

public class TestVacuumingPolicy
{
    [Fact]
    public void TestShouldExecute_Returns_True_When_It_Should_Execute()
    {
        VacuumingPolicy vacuumingPolicy = new()
        {
            Key = "Name",
            Description = "Description",
            Id = 0,
            Duration = "2y",
            LastExecution = DateTime.Now.AddYears(-2).AddDays(-1)
        };

        Assert.True(vacuumingPolicy.ShouldExecute());
    }

    [Fact]
    public void TestShouldExecute_Returns_False_When_It_Should_Not_Execute()
    {
        VacuumingPolicy vacuumingPolicy = new()
        {
            Key = "Name",
            Description = "Description",
            Id = 0,
            Duration = "5y",
            LastExecution = DateTime.Now.AddYears(-2).AddDays(-1)
        };

        Assert.False(vacuumingPolicy.ShouldExecute());
    }

    [Fact]
    public void TestToListingWorksWithNullValues()
    {
        VacuumingPolicy vacuumingPolicy = new()
        {
            Key = "Name",
            Purposes = new List<Purpose>()
        };

        Assert.Equal("Name, None, None, None, Empty", vacuumingPolicy.ToListing());
    }

    [Fact]
    public void SetDuration_Updates_Duration_If_Given_Valid_Duration()
    {
        VacuumingPolicy vacuumingPolicy = new()
        {
            Key = "Name",
            Purposes = new List<Purpose>()
        };

        vacuumingPolicy.Duration = "2y 5d";
        Assert.Equal("2y 5d", vacuumingPolicy.Duration);
    }

    [Fact]
    public void SetDuration_Throws_Exception_On_Invalid_Duration()
    {
        VacuumingPolicy vacuumingPolicy = new()
        {
            Key = "Name",
            Purposes = new List<Purpose>()
        };

        Assert.Throws<VacuumingPolicy.DurationParseException>(()=> vacuumingPolicy.Duration = "Test");
        
    }
}