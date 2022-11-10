using System.Linq;
using System.Text;
using GraphManipulation.Extensions;
using GraphManipulation.Models;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;
using VDS.RDF;
using VDS.RDF.Parsing;
using Xunit;

namespace Test;

public class StructureTest
{
    private const string baseUri = "http://www.test.com/";

    [Fact]
    public void StructuresWithSameNameUniqueUnderDifferentParentStructures()
    {
        const string tableName1 = "Table1";
        const string tableName2 = "Table2";
        const string columnName = "Column";

        var table1 = new Table(tableName1);
        var table2 = new Table(tableName2);
        var column1 = new Column(columnName);
        var column2 = new Column(columnName);

        Assert.Equal(column1, column2);

        table1.AddStructure(column1);
        table2.AddStructure(column2);

        Assert.NotEqual(column1, column2);
    }

    [Fact]
    public void UpdateStoreSetsStore()
    {
        var sqlite = new Sqlite("SQLite");
        var column = new Column("Column");

        sqlite.UpdateBaseUri(baseUri);
        column.UpdateStore(sqlite);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void UpdateStoreDoesNotAddStructureToStoreListOfStructures()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");

        sqlite.UpdateBaseUri(baseUri);
        table.UpdateStore(sqlite);

        Assert.DoesNotContain(table, sqlite.SubStructures);
    }

    [Fact]
    public void UpdateStoreUpdatesParentStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column = new Column("Column");

        sqlite.UpdateBaseUri(baseUri);

        table.AddStructure(column);
        column.UpdateStore(sqlite);

        Assert.Equal(sqlite, table.Store);
    }

    [Fact]
    public void UpdateStoreUpdatesSubStructuresStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");

        sqlite.UpdateBaseUri(baseUri);

        table.AddStructure(column1);
        table.AddStructure(column2);

        table.UpdateStore(sqlite);

        Assert.Equal(sqlite, column1.Store);
        Assert.Equal(sqlite, column2.Store);
    }

    [Fact]
    public void UpdateStoreUpdatesOwnAndParentId()
    {
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName = "Column";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string sqliteString = baseUri + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Entity.HashToId(expectedSqliteHash);

        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Entity.HashToId(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Entity.HashToId(expectedColumnHash);

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column = new Column(columnName);

        sqlite.UpdateBaseUri(baseUri);

        table.AddStructure(column);
        column.UpdateStore(sqlite);

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }

    [Fact]
    public void UpdateStoreUpdatesOwnAndSubStructuresId()
    {
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName1 = "Column1";
        const string columnName2 = "Column2";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string sqliteString = baseUri + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Entity.HashToId(expectedSqliteHash);

        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Entity.HashToId(expectedTableHash);

        const string columnString1 = tableString + columnName1;
        var expectedColumn1Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString1));
        var expectedColumn1String = Entity.HashToId(expectedColumn1Hash);

        const string columnString2 = tableString + columnName2;
        var expectedColumn2Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString2));
        var expectedColumn2String = Entity.HashToId(expectedColumn2Hash);

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column1 = new Column(columnName1);
        var column2 = new Column(columnName2);

        sqlite.UpdateBaseUri(baseUri);

        table.AddStructure(column1);
        table.AddStructure(column2);

        table.UpdateStore(sqlite);

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumn1String, column1.Id);
        Assert.Equal(expectedColumn2String, column2.Id);
    }

    [Fact]
    public void UpdateBaseUpdatesParentsBase()
    {
        var table = new Table("Table");
        var column = new Column("Column");

        table.AddStructure(column);
        column.UpdateBaseUri("http://www.expected.com/");

        Assert.Equal("http://www.expected.com/", column.BaseUri);
        Assert.Equal(column.BaseUri, table.BaseUri);
    }

    [Fact]
    public void UpdateBaseUpdatesStoreBase()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column = new Column("Column");

        sqlite.UpdateBaseUri(baseUri);
        sqlite.AddStructure(table);
        table.AddStructure(column);
        column.UpdateBaseUri("http://www.expected.com/");

        Assert.Equal("http://www.expected.com/", sqlite.BaseUri);
    }

    [Fact]
    public void UpdateBaseUpdatesSubStructuresBase()
    {
        var table = new Table("Table");
        var column1 = new Column("Column1");
        var column2 = new Column("Column2");

        table.AddStructure(column1);
        table.AddStructure(column2);

        table.UpdateBaseUri(baseUri);

        Assert.Equal(baseUri, table.BaseUri);
        Assert.Equal(table.BaseUri, column1.BaseUri);
        Assert.Equal(table.BaseUri, column2.BaseUri);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndParentId()
    {
        const string tableName = "Table";
        const string columnName = "Column";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string tableString = baseUri + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Entity.HashToId(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Entity.HashToId(expectedColumnHash);

        var table = new Table(tableName);
        var column = new Column(columnName);

        table.AddStructure(column);
        column.UpdateBaseUri(baseUri);

        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }

    [Fact]
    public void UpdateBaseUpdatesOwnAndStoreId()
    {
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName = "Column";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string sqliteString = baseUri + sqliteName;
        var expectedSqliteHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(sqliteString));
        var expectedSqliteString = Entity.HashToId(expectedSqliteHash);

        const string tableString = sqliteString + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Entity.HashToId(expectedTableHash);

        const string columnString = tableString + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Entity.HashToId(expectedColumnHash);

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column = new Column(columnName);

        sqlite.UpdateBaseUri(baseUri + "/Test/");

        sqlite.AddStructure(table);
        table.AddStructure(column);
        column.UpdateBaseUri(baseUri);

        Assert.Equal(expectedSqliteString, sqlite.Id);
        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumnString, column.Id);
    }

    [Fact]
    public void UpdateBaseUpdatesSubStructuresId()
    {
        const string tableName = "Table";
        const string columnName1 = "Column1";
        const string columnName2 = "Column2";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string tableString = baseUri + tableName;
        var expectedTableHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(tableString));
        var expectedTableString = Entity.HashToId(expectedTableHash);

        const string columnString1 = tableString + columnName1;
        var expectedColumn1Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString1));
        var expectedColumn1String = Entity.HashToId(expectedColumn1Hash);

        const string columnString2 = tableString + columnName2;
        var expectedColumn2Hash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString2));
        var expectedColumn2String = Entity.HashToId(expectedColumn2Hash);

        var table = new Table(tableName);
        var column1 = new Column(columnName1);
        var column2 = new Column(columnName2);

        table.AddStructure(column1);
        table.AddStructure(column2);

        table.UpdateBaseUri(baseUri);

        Assert.Equal(expectedTableString, table.Id);
        Assert.Equal(expectedColumn1String, column1.Id);
        Assert.Equal(expectedColumn2String, column2.Id);
    }

    [Fact]
    public void AddStructureAddsGivenStructureToListOfStructures()
    {
        const string schemaName = "Schema";
        const string tableName = "Table";

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        schema.AddStructure(table);
        Assert.Contains(table, schema.SubStructures);
    }

    [Fact]
    public void StructureCannotBeAddedMultipleTimesToStructureListOfSubStructures()
    {
        const string schemaName = "Schema";
        const string tableName = "Table";

        var schema = new Schema(schemaName);
        var table = new Table(tableName);

        schema.AddStructure(table);
        schema.AddStructure(table);
        Assert.Single(schema.SubStructures);
    }

    [Fact]
    public void AddStructureSetsSubStructureStore()
    {
        const string sqliteName = "SQLite";
        const string tableName = "Table";
        const string columnName = "Column";

        var sqlite = new Sqlite(sqliteName);
        var table = new Table(tableName);
        var column = new Column(columnName);

        sqlite.UpdateBaseUri(baseUri);
        sqlite.AddStructure(table);

        table.AddStructure(column);

        Assert.Equal(sqlite, table.Store);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void AddStructureUpdatesBase()
    {
        var table = new Table("Table");
        var column = new Column("Column");

        table.UpdateBaseUri(baseUri);
        table.AddStructure(column);

        Assert.Equal(baseUri, table.BaseUri);
        Assert.Equal(table.BaseUri, column.BaseUri);
    }

    [Fact]
    public void AddStructureUpdatesStore()
    {
        var sqlite = new Sqlite("SQLite");
        var table = new Table("Table");
        var column = new Column("Column");

        sqlite.UpdateBaseUri(baseUri);
        sqlite.AddStructure(table);
        table.AddStructure(column);

        Assert.Equal(sqlite, table.Store);
        Assert.Equal(sqlite, column.Store);
    }

    [Fact]
    public void StructureWithBaseIdWithBase()
    {
        const string baseNamespace = "http://www.expected.com/";
        const string columnName = "Column";

        var algorithm = EntityTest.GetHashAlgorithm();

        const string columnString = baseNamespace + columnName;
        var expectedColumnHash = algorithm.ComputeHash(Encoding.ASCII.GetBytes(columnString));
        var expectedColumnString = Entity.HashToId(expectedColumnHash);

        var column = new Column("Column");
        column.UpdateBaseUri(baseNamespace);

        Assert.Equal(expectedColumnString, column.Id);
        Assert.Equal("http://www.expected.com/Column", column.HashedFrom);
    }

    [Fact]
    public void AddStructureStructureRemovedFromParentStructuresSubStructures()
    {
        var table1 = new Table("Table1");
        var table2 = new Table("Table2");
        var column = new Column("Column");

        table1.UpdateBaseUri(baseUri);
        table2.UpdateBaseUri(baseUri);
        table1.AddStructure(column);

        Assert.Equal(column, table1.SubStructures.First());
        Assert.Equal(table1, column.ParentStructure);

        table2.AddStructure(column);

        Assert.Empty(table1.SubStructures);
        Assert.Equal(column, table2.SubStructures.First());
        Assert.Equal(table2, column.ParentStructure);
    }


    public class SchemaTest
    {
    }

    public class TableTest
    {
        [Fact]
        public void AddPrimaryKeyAddsPrimaryKey()
        {
            var table = new Table("Table");
            var column = new Column("Column");

            table.UpdateBaseUri(baseUri);
            table.AddStructure(column);
            table.AddPrimaryKey(column);

            Assert.Contains(column, table.SubStructures);
        }

        [Fact]
        public void AddPrimaryKeyNotInSubStructuresThrowsException()
        {
            var table = new Table("Table");
            var column = new Column("Column");

            table.UpdateBaseUri(baseUri);

            Assert.Throws<StructureException>(() => table.AddPrimaryKey(column));
        }

        [Fact]
        public void AddPrimaryKeyAlreadyExistsNotAdded()
        {
            var table = new Table("Table");
            var column = new Column("Column");

            table.UpdateBaseUri(baseUri);
            table.AddStructure(column);
            table.AddPrimaryKey(column);
            table.AddPrimaryKey(column);

            Assert.Single(table.PrimaryKeys);
        }

        [Fact]
        public void AddForeignKeyAddsForeignKey()
        {
            var table1 = new Table("Table1");
            var column1 = new Column("Column1");

            var table2 = new Table("Table2");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table1.AddStructure(column1);

            table2.UpdateBaseUri(baseUri);
            table2.AddStructure(column2);

            var foreignKey = new ForeignKey(column1, column2);

            table1.AddForeignKey(foreignKey);

            Assert.Contains(foreignKey, table1.ForeignKeys);
        }

        [Fact]
        public void AddForeignKeyFromColumnReferencesToColumn()
        {
            var table1 = new Table("Table1");
            var column1 = new Column("Column1");

            var table2 = new Table("Table2");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table1.AddStructure(column1);

            table2.UpdateBaseUri(baseUri);
            table2.AddStructure(column2);

            table1.AddForeignKey(column1, column2);

            Assert.Equal(column2, table1.ForeignKeys.First().To);
        }

        [Fact]
        public void AddForeignKeyFromColumnNotInSubStructuresThrowsException()
        {
            var table1 = new Table("Table1");
            var column1 = new Column("Column1");

            var table2 = new Table("Table2");
            var column2 = new Column("Column2");

            var table3 = new Table("Table3");

            table1.UpdateBaseUri(baseUri);

            table2.UpdateBaseUri(baseUri);
            table2.AddStructure(column2);

            table3.UpdateBaseUri(baseUri);
            table3.AddStructure(column1);

            Assert.Throws<StructureException>(() => table1.AddForeignKey(column1, column2));
        }

        [Fact]
        public void AddForeignKeyAlreadyExistsNotAdded()
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table1.AddStructure(column1);

            table2.UpdateBaseUri(baseUri);
            table2.AddStructure(column2);

            table1.AddForeignKey(column1, column2);
            table1.AddForeignKey(column1, column2);

            Assert.Single(table1.ForeignKeys);
        }

        [Fact]
        public void AddForeignKeySameToTableDifferentOnActionThrowsException()
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            var column11 = new Column("Column11");
            var column12 = new Column("Column12");
            var column21 = new Column("Column21");
            var column22 = new Column("Column22");

            table1.UpdateBaseUri(baseUri);
            table1.AddStructure(column11);
            table1.AddStructure(column12);

            table2.UpdateBaseUri(baseUri);
            table2.AddStructure(column21);
            table2.AddStructure(column22);

            table1.AddForeignKey(new ForeignKey(column11, column21, ForeignKeyOnEnum.Cascade));
            Assert.Throws<StructureException>(() =>
                table1.AddForeignKey(new ForeignKey(column12, column22)));
        }

        [Fact]
        public void DeleteForeignKeyByNameRemovesForeignKey()
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table2.UpdateBaseUri(baseUri);

            table1.AddStructure(column1);
            table2.AddStructure(column2);

            table1.AddForeignKey(column1, column2);

            table1.DeleteForeignKey(column1.Name);
            Assert.Empty(table1.ForeignKeys);
        }

        [Fact]
        public void DeleteForeignKeyByColumnRemovesForeignKey()
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table2.UpdateBaseUri(baseUri);

            table1.AddStructure(column1);
            table2.AddStructure(column2);

            table1.AddForeignKey(column1, column2);

            table1.DeleteForeignKey(column1);
            Assert.Empty(table1.ForeignKeys);
        }

        [Fact]
        public void DeleteForeignKeyByForeignKeyRemovesForeignKey()
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table2.UpdateBaseUri(baseUri);

            table1.AddStructure(column1);
            table2.AddStructure(column2);

            var foreignKey = new ForeignKey(column1, column2);
            table1.AddForeignKey(foreignKey);

            table1.DeleteForeignKey(foreignKey);
            table1.DeleteForeignKey(foreignKey);
            Assert.Empty(table1.ForeignKeys);
        }

        [Fact]
        public void ToGraphNoPrimaryKeyThrowsException()
        {
            var table = new Table("Table");
            table.UpdateBaseUri(baseUri);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(table);

            Assert.Throws<DataStoreToGraphException>(() => table.ToGraph());
        }

        [Fact]
        public void ToGraphPrimaryKeysAddedToGraph()
        {
            var table = new Table("Table");
            var column = new Column("Column");

            table.UpdateBaseUri(baseUri);
            table.AddStructure(column);
            table.AddPrimaryKey(column);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(table);

            var graph = table.ToGraph();

            var subj = graph.CreateUriNode(table.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.PrimaryKey);
            var obj = graph.CreateUriNode(column.Uri);

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphForeignKeysAddedToGraph()
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            var column1 = new Column("Column1");
            var column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table2.UpdateBaseUri(baseUri);

            table1.AddStructure(column1);
            table2.AddStructure(column2);

            table1.AddPrimaryKey(column1);
            table2.AddPrimaryKey(column2);

            var foreignKey = new ForeignKey(column1, column2);
            table1.AddForeignKey(foreignKey);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(table1);
            sqlite.AddStructure(table2);

            var graph = table1.ToGraph();

            var subj = graph.CreateUriNode(table1.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.ForeignKey);
            var obj = graph.CreateUriNode(column1.Uri);

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphForeignKeyReferencesAddedToGraph()
        {
            var graph = GenerateGraphWithForeignKey(
                ForeignKeyOnEnum.Cascade,
                ForeignKeyOnEnum.NoAction,
                out var column1, out var column2);

            var subj = graph.CreateUriNode(column1.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.References);
            var obj = graph.CreateUriNode(column2.Uri);

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphForeignKeyOnDeleteCascadeAddedToGraph()
        {
            var graph = GenerateGraphWithForeignKey(
                ForeignKeyOnEnum.Cascade,
                ForeignKeyOnEnum.NoAction,
                out var column1, out var column2);

            var subj = graph.CreateUriNode(column1.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.ForeignKeyOnDelete);
            var obj = graph.CreateLiteralNode("CASCADE");

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphForeignKeyOnDeleteNoActionAddedToGraph()
        {
            var graph = GenerateGraphWithForeignKey(
                ForeignKeyOnEnum.NoAction,
                ForeignKeyOnEnum.NoAction,
                out var column1, out var column2);

            var subj = graph.CreateUriNode(column1.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.ForeignKeyOnDelete);
            var obj = graph.CreateLiteralNode("NO ACTION");

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphForeignKeyOnUpdateCascadeAddedToGraph()
        {
            var graph = GenerateGraphWithForeignKey(
                ForeignKeyOnEnum.NoAction,
                ForeignKeyOnEnum.Cascade,
                out var column1, out var column2);

            var subj = graph.CreateUriNode(column1.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.ForeignKeyOnUpdate);
            var obj = graph.CreateLiteralNode("CASCADE");

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphForeignKeyOnUpdateNoActionAddedToGraph()
        {
            var graph = GenerateGraphWithForeignKey(
                ForeignKeyOnEnum.NoAction,
                ForeignKeyOnEnum.NoAction,
                out var column1, out var column2);

            var subj = graph.CreateUriNode(column1.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.ForeignKeyOnUpdate);
            var obj = graph.CreateLiteralNode("NO ACTION");

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        private IGraph GenerateGraphWithForeignKey(ForeignKeyOnEnum onDelete, ForeignKeyOnEnum onUpdate,
            out Column column1, out Column column2)
        {
            var table1 = new Table("Table1");
            var table2 = new Table("Table2");
            column1 = new Column("Column1");
            column2 = new Column("Column2");

            table1.UpdateBaseUri(baseUri);
            table2.UpdateBaseUri(baseUri);

            table1.AddStructure(column1);
            table2.AddStructure(column2);

            table1.AddPrimaryKey(column1);
            table2.AddPrimaryKey(column2);

            var foreignKey = new ForeignKey(column1, column2, onDelete, onUpdate);
            table1.AddForeignKey(foreignKey);

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(table1);
            sqlite.AddStructure(table2);

            var graph = table1.ToGraph();

            return graph;
        }
    }

    public class ColumnTest
    {
        [Fact]
        public void SetDataTypeSetsDataType()
        {
            var column = new Column("Column");

            column.SetDataType("Test");

            Assert.Equal("Test", column.DataType);
        }

        [Fact]
        public void SetIsNotNullSetIsNotNull()
        {
            var column = new Column("Column");
            column.SetIsNotNull(true);
            Assert.True(column.IsNotNull);
        }

        [Fact]
        public void SetOptionsSetsOptions()
        {
            var column = new Column("Column");
            column.SetOptions("Options");
            Assert.Equal("Options", column.Options);
        }

        [Fact]
        public void ToGraphDataTypeAddedToGraph()
        {
            var column = new Column("Column");
            column.UpdateBaseUri(baseUri);
            column.SetDataType("Test");

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.HasDataType);
            var obj = graph.CreateLiteralNode("Test");

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphIsNotNullAddedToGraph()
        {
            var column = new Column("MyColumn");
            column.UpdateBaseUri(baseUri);
            column.SetDataType("INT");

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.IsNotNull);
            var obj = graph.CreateLiteralNode("false", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeBoolean));

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }

        [Fact]
        public void ToGraphOptionsAddedToGraph()
        {
            var column = new Column("MyColumn");
            column.UpdateBaseUri(baseUri);
            column.SetDataType("INT");
            column.SetOptions("AUTOINCREMENT");

            // Added to avoid StructureException caused by Structure having no Store
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);
            sqlite.AddStructure(column);

            var graph = column.ToGraph();

            var subj = graph.CreateUriNode(column.Uri);
            var pred = graph.CreateUriNode(DataStoreDescriptionLanguage.ColumnOptions);
            var obj = graph.CreateLiteralNode("AUTOINCREMENT");

            var triple = new Triple(subj, pred, obj);

            Assert.Contains(triple, graph.Triples);
        }
    }
}