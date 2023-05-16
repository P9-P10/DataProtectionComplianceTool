using GraphManipulation.Utility;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

class TestDbContext : DbContext
{
}

public class DatabaseInitiatorTest
{
    [Fact]
    public void TestReplaceCreateTableWithCreateTableIfNotExists()
    {
        string output = CreateStatementManipulator.ReplaceCreateTableWithCreateTableIfNotExists("CREATE TABLE A");
        
        Assert.Equal("CREATE TABLE IF NOT EXISTS A",output);

    }
    
    [Fact]
    public void TestReplaceCreateIndexWithCreateIndexIfNotExists()
    {
        string output = CreateStatementManipulator.ReplaceCreateIndexWithCreateIndexIfNotExists("CREATE INDEX A");
        
        Assert.Equal("CREATE INDEX IF NOT EXISTS A",output);

    }
}