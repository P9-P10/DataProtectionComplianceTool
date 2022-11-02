using System;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;
using Xunit.Abstractions;

namespace Test;

public class TransformationsTest
{
    public class ToSqlCreateStatement
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ToSqlCreateStatement(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Column()
        {
            var column = new Column("TestColumn", "int");
            var actualString = column.ToSqlCreateStatement();
            var expectedString = "TestColumn INT";
            Assert.Equal(expectedString, actualString);
        }

        [Fact]
        public void ColumnIsNotNull()
        {
            var column = new Column("MyColumn", "int", true);
            var actual = column.ToSqlCreateStatement();
            var expected = "MyColumn INT NOT NULL";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TableWithColumns()
        {
            var table = new Table("MyTable");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");
            
            table.AddStructure(column1);
            table.AddStructure(column2);
            
            table.AddPrimaryKey(column1);
            
            column1.SetDataType("int");
            column2.SetDataType("varchar");

            var actual = table.ToSqlCreateStatement();
            var expected = "CREATE TABLE MyTable (\n\tColumn1 INT,\n\tColumn2 VARCHAR,\n\tPRIMARY KEY (Column1)\n);";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TableWithPrimaryKeys()
        {
            
        }

        [Fact]
        public void TableWithForeignKeysSingle()
        {
            
        }

        [Fact]
        public void TableWithForeignKeysComposite()
        {
            
        }

        [Fact]
        public void TableWithForeignKeysCompositeAndSingle()
        {
            
        }

        [Fact]
        public void StructuredEntityEmpty()
        {
            var sqlite = new Sqlite("TestSqlite");
            var actual = sqlite.ToSqlCreateStatement();
            var expected = "";
            
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StructuredEntityWithSubStructures()
        {
            var sqlite = new Sqlite("MySqlite", "http://www.test.com/");
            
            
            var table1 = new Table("MyTable1");
            var column11 = new Column("Column11", "daTeTimE", true);
            var column12 = new Column("Column12", "varchar(255)");
            var column13 = new Column("Column13", "int");
            
            var table2 = new Table("MyTable2");
            var column21 = new Column("Column21", "int");
            var column22 = new Column("Column22", "varchar", true);

            var table3 = new Table("MyTable3");
            var column31 = new Column("Column31", "varchar(255)");
            var column32 = new Column("Column32", "int");

            sqlite.AddStructure(table1);
            table1.AddStructure(column11);
            table1.AddStructure(column12);
            table1.AddStructure(column13);
            
            table1.AddPrimaryKey(column11);

            sqlite.AddStructure(table2);
            table2.AddStructure(column21);
            table2.AddStructure(column22);
            
            table2.AddPrimaryKey(column22);
            
            sqlite.AddStructure(table3);
            table3.AddStructure(column31);
            table3.AddStructure(column32);
            
            table3.AddPrimaryKey(column31);
            table3.AddPrimaryKey(column32);
            
            table1.AddForeignKey(column11, column21);
            table1.AddForeignKey(column12, column31);
            table1.AddForeignKey(column13, column32);
        
            var actual = sqlite.ToSqlCreateStatement();

            var expected = "CREATE TABLE MyTable1 " +
                           "(\n\tColumn11 DATETIME NOT NULL," +
                           "\n\tColumn12 VARCHAR(255)," +
                           "\n\tColumn13 INT," +
                           "\n\tPRIMARY KEY (Column11)," +
                           "\n\tFOREIGN KEY (Column11) REFERENCES MyTable2 (Column21)," +
                           "\n\tFOREIGN KEY (Column12, Column13) REFERENCES MyTable3 (Column31, Column32)\n);\n" +
                           "CREATE TABLE MyTable2 " +
                           "(\n\tColumn21 INT," +
                           "\n\tColumn22 VARCHAR NOT NULL," +
                           "\n\tPRIMARY KEY (Column22)\n);\n" +
                           "CREATE TABLE MyTable3 " +
                           "(\n\tColumn31 VARCHAR(255)," +
                           "\n\tColumn32 INT," +
                           "\n\tPRIMARY KEY (Column31, Column32)\n);";

            Assert.Equal(expected, actual);
        }
    }
}