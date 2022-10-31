using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class SqliteTest
{
    private const string BaseUri = "http://www.test.com/";

    public class TestDatabaseFixture : IDisposable
    {
        private static readonly string _testDatabase = $"TestResources{Path.DirectorySeparatorChar}SimpleDatabase.sqlite";

        private static string TestDatabase
        {
            get
            {
                if (!File.Exists(_testDatabase))
                {
                    throw new FileNotFoundException("File not found: " + _testDatabase);
                }
                
                return _testDatabase;
            } 
        }
        
        private static readonly SQLiteConnection Connection = new($"Data Source={TestDatabase}");

        public readonly Sqlite Sqlite;
        
        public readonly Sqlite ExpectedSqlite = new("SimpleDatabase", BaseUri);
        public readonly Schema ExpectedSchema = new("main");
        public readonly Table ExpectedTableUsers = new("Users");
        public readonly Table ExpectedTableUserData = new("UserData");
        public readonly Column ExpectedColumnUsersId = new("id");
        public readonly Column ExpectedColumnUserDataId = new("id");
        public readonly Column ExpectedColumnEmail = new ("email");
        public readonly Column ExpectedColumnPhone = new ("phone");


        public TestDatabaseFixture()
        {
            Sqlite = new Sqlite("", BaseUri, Connection);
            
            Sqlite.Build();
            
            ExpectedSqlite.AddStructure(ExpectedSchema);
            
            ExpectedSchema.AddStructure(ExpectedTableUsers);
            ExpectedSchema.AddStructure(ExpectedTableUserData);
            
            ExpectedTableUsers.AddStructure(ExpectedColumnUsersId);
            ExpectedTableUsers.AddStructure(ExpectedColumnEmail);
            
            ExpectedTableUserData.AddStructure(ExpectedColumnUserDataId);
            ExpectedTableUserData.AddStructure(ExpectedColumnPhone);
            
            ExpectedTableUsers.AddPrimaryKey(ExpectedColumnUsersId);
            ExpectedTableUserData.AddPrimaryKey(ExpectedColumnUserDataId);
            
            ExpectedTableUserData.AddForeignKey(ExpectedColumnUserDataId, ExpectedColumnUsersId);
            
            ExpectedColumnEmail.SetDataType("VARCHAR");
            ExpectedColumnPhone.SetDataType("INT");
        }
        
        public void Dispose()
        {
            
        }
    }

    public class BuildTest : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _testDatabaseFixture;

        public BuildTest(TestDatabaseFixture testDatabaseFixture)
        {
            _testDatabaseFixture = testDatabaseFixture;
        }


        [Fact]
        public void WithoutConnectionThrowsException()
        {
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(BaseUri);

            Assert.Throws<DataStoreException>(() => sqlite.Build());
        }

        [Fact]
        public void SqliteGetsAName()
        {
            Assert.Equal("SimpleDatabase", _testDatabaseFixture.Sqlite.Name);
        }

        private Schema GetSchemaFromDatastore( string schemaName, DataStore store)
        {
            return (store.SubStructures.First(s => s.Name == schemaName) as Schema)!;
        }

        [Fact]
        public void SqliteGetsSchemas()
        {
            var actualSchema = GetSchemaFromDatastore("main", _testDatabaseFixture.Sqlite);
            
            Assert.Equal(_testDatabaseFixture.ExpectedSchema, actualSchema);
            Assert.IsType<Schema>(actualSchema);
        }

        private Table GetTableFromSchema(string tableName, Schema schema)
        {
            return (schema.SubStructures.First(s => s.Name == tableName) as Table)!;
        }

        [Fact]
        public void SqliteGetsTables()
        {
            var actualUsersTable =
                GetTableFromSchema("Users", 
                    GetSchemaFromDatastore("main", 
                        _testDatabaseFixture.Sqlite));

            var actualUserDataTable =
                GetTableFromSchema("UserData", 
                    GetSchemaFromDatastore("main", 
                        _testDatabaseFixture.Sqlite));

            Assert.Equal(_testDatabaseFixture.ExpectedTableUsers, actualUsersTable);
            Assert.Equal(_testDatabaseFixture.ExpectedTableUserData, actualUserDataTable);
            Assert.IsType<Table>(actualUsersTable);
            Assert.IsType<Table>(actualUserDataTable);
        }

        private Column GetColumnFromTable(string columnName, Table table)
        {
            return (table.SubStructures.First(s => s.Name == columnName) as Column)!;
        }

        [Fact]
        public void SqliteGetsColumn()
        {
            var actualColumnEmail = 
                GetColumnFromTable("email", 
                    GetTableFromSchema("Users", 
                        GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)));
            

            var actualColumnPhone = 
                GetColumnFromTable("phone", 
                    GetTableFromSchema("UserData", 
                        GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)));

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail, actualColumnEmail);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone, actualColumnPhone);
            Assert.IsType<Column>(actualColumnEmail);
            Assert.IsType<Column>(actualColumnPhone);
        }

        [Fact]
        public void SqliteColumnsGetDataType()
        {
            var actualColumnEmailDataType = 
                GetColumnFromTable("email", 
                    GetTableFromSchema("Users", 
                        GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)))
                    .DataType;

            var actualColumnPhoneDataType =  
                GetColumnFromTable("phone", 
                        GetTableFromSchema("UserData", 
                            GetSchemaFromDatastore("main", 
                                _testDatabaseFixture.Sqlite)))
                    .DataType;

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail.DataType, actualColumnEmailDataType);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone.DataType, actualColumnPhoneDataType);
        }

        [Fact]
        public void SqliteTableGetsPrimaryKeys()
        {
            var actualUsersPrimaryKey =
                GetTableFromSchema("Users",
                    GetSchemaFromDatastore("main",
                        _testDatabaseFixture.Sqlite))
                    .PrimaryKeys
                    .First(c => c.Name == "id");

            var actualUserDataPrimaryKey = 
                GetTableFromSchema("UserData",
                        GetSchemaFromDatastore("main",
                            _testDatabaseFixture.Sqlite))
                    .PrimaryKeys
                    .First(c => c.Name == "id");

            Assert.Equal(_testDatabaseFixture.ExpectedColumnUsersId, actualUsersPrimaryKey);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnUserDataId, actualUserDataPrimaryKey);
        }

        [Fact]
        public void SqliteTableGetsForeignKeys()
        {
            var actualForeignKey =
                GetTableFromSchema("UserData",
                        GetSchemaFromDatastore("main",
                            _testDatabaseFixture.Sqlite))
                    .ForeignKeys
                    .First(c => c.Name == "id");
            
            Assert.Equal(_testDatabaseFixture.ExpectedColumnUserDataId, actualForeignKey);
        }

        [Fact]
        public void SqliteForeignKeyColumnsReferencesColumns()
        {
            
        }
    }
}