using GraphManipulation.MetadataManagement;
using Xunit;

namespace Test.Metadata;

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
        builder.InsertValues = new GDPRMetadata("mockTable", "mockColumn")
            { TargetTable = "test_table", TargetColumn = "test_column", Purpose = "Testing", LegallyRequired = false};

        var result = builder.Build();

        Assert.Equal(
            "INSERT INTO gdpr_metadata (purpose, target_table, target_column, legally_required) VALUES('Testing', 'test_table', 'test_column', false);",
            result);
    }
}