using GraphManipulation.Models;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class ForeignKeyTest
{
    [Fact]
    public void FromParentNullThrowsException()
    {
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");

        var table2 = new Table("Table2");
        table2.AddStructure(column2);
        
        Assert.Throws<ForeignKeyException>(() => new ForeignKey(column1, column2));
    }

    [Fact]
    public void ToParentNullThrowsException()
    {
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");

        var table1 = new Table("Table1");
        table1.AddStructure(column1);
        
        Assert.Throws<ForeignKeyException>(() => new ForeignKey(column1, column2));
    }

    [Fact]
    public void SameParentsThrowsException()
    {
        var table = new Table("Table");
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");
        
        table.AddStructure(column1);
        table.AddStructure(column2);

        Assert.Throws<ForeignKeyException>(() => new ForeignKey(column1, column2));
    }

    [Fact]
    public void ForeignKeyCreatedFromAndToHaveValuesWithOnAction()
    {
        var table1 = new Table("Table1");
        var table2 = new Table("Table2");
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");
        
        table1.AddStructure(column1);
        table2.AddStructure(column2);

        var foreignKey = new ForeignKey(column1, column2, onUpdate: ForeignKeyOnEnum.Cascade);
        Assert.Equal(column1, foreignKey.From);
        Assert.Equal(column2, foreignKey.To);
        Assert.Equal(table1, foreignKey.From.ParentStructure);
        Assert.Equal(table2, foreignKey.To.ParentStructure);
        Assert.Equal(ForeignKeyOnEnum.Cascade, foreignKey.OnUpdate);
        Assert.Equal(ForeignKeyOnEnum.NoAction, foreignKey.OnDelete);
    }
}