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

        [Fact]
        public void SqliteGetsSchemas()
        {
            var actualSchema = Schema.GetSchemaFromDatastore("main", _testDatabaseFixture.Sqlite);
            
            Assert.Equal(_testDatabaseFixture.ExpectedSchema, actualSchema);
        }
        
        [Fact]
        public void SqliteGetsTables()
        {
            var actualUsersTable =
                Table.GetTableFromSchema("Users", 
                    Schema.GetSchemaFromDatastore("main", 
                        _testDatabaseFixture.Sqlite));

            var actualUserDataTable =
                Table.GetTableFromSchema("UserData", 
                    Schema.GetSchemaFromDatastore("main", 
                        _testDatabaseFixture.Sqlite));

            Assert.Equal(_testDatabaseFixture.ExpectedTableUsers, actualUsersTable);
            Assert.Equal(_testDatabaseFixture.ExpectedTableUserData, actualUserDataTable);
        }

        [Fact]
        public void SqliteGetsColumn()
        {
            var actualColumnEmail = 
                Column.GetColumnFromTable("email", 
                    Table.GetTableFromSchema("Users", 
                        Schema.GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)));
            

            var actualColumnPhone = 
                Column.GetColumnFromTable("phone", 
                    Table.GetTableFromSchema("UserData", 
                        Schema.GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)));

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail, actualColumnEmail);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone, actualColumnPhone);
        }

        [Fact]
        public void SqliteColumnsGetDataType()
        {
            var actualColumnEmailDataType = 
                Column.GetColumnFromTable("email", 
                    Table.GetTableFromSchema("Users", 
                        Schema.GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)))
                    .DataType;

            var actualColumnPhoneDataType =  
                Column.GetColumnFromTable("phone", 
                    Table.GetTableFromSchema("UserData", 
                        Schema.GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)))
                    .DataType;

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail.DataType, actualColumnEmailDataType);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone.DataType, actualColumnPhoneDataType);
        }

        [Fact]
        public void SqliteTableGetsPrimaryKeys()
        {
            var actualUsersPrimaryKey =
                Table.GetTableFromSchema("Users",
                    Schema.GetSchemaFromDatastore("main",
                        _testDatabaseFixture.Sqlite))
                    .PrimaryKeys
                    .First(c => c.Name == "id");

            var actualUserDataPrimaryKey = 
                Table.GetTableFromSchema("UserData",
                        Schema.GetSchemaFromDatastore("main",
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
                Table.GetTableFromSchema("UserData",
                        Schema.GetSchemaFromDatastore("main",
                            _testDatabaseFixture.Sqlite))
                    .ForeignKeys
                    .First(c => c.Name == "id");
            
            Assert.Equal(_testDatabaseFixture.ExpectedColumnUserDataId, actualForeignKey);
        }

        [Fact]
        public void SqliteForeignKeyColumnsReferencesColumns()
        {
            var actualReferencedColumn =
                Column.GetColumnFromTable("id", 
                    Table.GetTableFromSchema("Users", 
                        Schema.GetSchemaFromDatastore("main", 
                            _testDatabaseFixture.Sqlite)));
            
            Assert.Equal(_testDatabaseFixture.ExpectedColumnUserDataId.References, actualReferencedColumn);
        }
    }
}