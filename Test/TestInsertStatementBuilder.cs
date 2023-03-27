using GraphManipulation.Helpers;
using GraphManipulation.Models.Metadata;
using Xunit;

namespace Test;

public class TestInsertStatementBuilder
{
    [Fact]
    public void BuildInsertFromAnonymousType()
    {
        var builder = new InsertStatementBuilder("test_table");
        builder.InsertValues = new { value_one = "one", value_two = 2, value_three = false };

        var result = builder.Build();

        Assert.Equal("INSERT INTO test_table (value_one, value_two, value_three) VALUES('one', 2, false);", result);
    }

    [Fact]
    public void BuildInsertFromNonAnonymousType()
    {
        var builder = new InsertStatementBuilder("gdpr_metadata");
        builder.InsertValues = new GDPRMetadata
            { target_table = "test_table", target_column = "test_column", purpose = "Testing" };

        var result = builder.Build();

        Assert.Equal(
            "INSERT INTO gdpr_metadata (purpose, target_table, target_column) VALUES('Testing', 'test_table', 'test_column');",
            result);
    }
}