using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class StructureNavigationTest
{
    [Fact]
    public void BaseTest()
    {
        var sqlite = new Sqlite("Sqlite", "http://www.test.com/");
        var schema = new Schema("Schema");
        var table1 = new Table("Table1");
        var table2 = new Table("Table2");
        var column11 = new Column("Column11");
        var column12 = new Column("Column12");
        var column21 = new Column("Column21");
        var column22 = new Column("Column22");
        
        sqlite.AddStructure(schema);
        schema.AddStructure(table1);
        schema.AddStructure(table2);
        table1.AddStructure(column11);
        table1.AddStructure(column12);
        table2.AddStructure(column21);
        table2.AddStructure(column22);

        Assert.Equal(column11, 
            sqlite
                .FindSchema("Schema")
                .FindTable("Table1")
                .FindColumn("Column11"));
        
        Assert.Equal(column22, 
            sqlite
                .FindSchema("Schema")
                .FindTable("Table2")
                .FindColumn("Column22"));
    }
}