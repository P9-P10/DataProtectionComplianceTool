using GraphManipulation.SchemaEvolution.Models.Structures;
using Xunit;

namespace Test.SchemaEvolution;

public class NamedEntityTest
{
    private const string BaseUri = "http://www.test.com/";

    [Fact]
    public void UpdateNameSetsName()
    {
        var column = new Column("Column");
        var newName = "NewColumnName";

        column.UpdateName(newName);

        Assert.Equal(newName, column.Name);
    }

    [Fact]
    public void UpdateNameUpdatesId()
    {
        var column = new Column("Column");
        var newName = "NewColumnName";

        column.UpdateName(newName);

        var expectedColumn = new Column(newName);

        Assert.Equal(expectedColumn.Id, column.Id);
    }

    [Fact]
    public void UpdateNameUpdatesChildrenId()
    {
        var table = new Table("Table");
        var column = new Column("Column");

        var newTableName = "NewTableName";

        table.AddStructure(column);

        table.UpdateName(newTableName);

        var expectedTable = new Table(newTableName);
        var expectedColumn = new Column("Column");

        expectedTable.AddStructure(expectedColumn);

        Assert.Equal(expectedColumn.Id, column.Id);
    }
}