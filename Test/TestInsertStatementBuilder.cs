using System;
using System.Collections.Generic;
using System.Reflection;
using GraphManipulation.Helpers;
using GraphManipulation.Models.Metadata;
using Xunit;

namespace Test;

public class TestInsertStatementBuilder
{
    [Fact]
    public void BuildInsertFromAnonymousType()
    {
        InsertStatementBuilder builder = new InsertStatementBuilder("test_table");
        builder.InsertValues = new { value_one = "one", value_two = 2, value_three = false };

        string result = builder.Build();
        
        Assert.Equal("INSERT INTO test_table (value_one, value_two, value_three) VALUES('one', 2, false);", result);
    }
    
    [Fact]
    public void BuildInsertFromNonAnonymousType()
    {
        InsertStatementBuilder builder = new InsertStatementBuilder("gdpr_metadata");
        builder.InsertValues = new GDPRMetadata() { target_table = "test_table", target_column = "test_column", purpose = "Testing" };

        string result = builder.Build();
        
        Assert.Equal("INSERT INTO gdpr_metadata (purpose, target_table, target_column) VALUES('Testing', 'test_table', 'test_column');", result);
    }
}