using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class SqliteTest
{
    private const string baseUri = "http://www.test.com/";

    public class BuildTest
    {
        private readonly string _testDatabase = $"TestResources{Path.DirectorySeparatorChar}SimpleDatabase.sqlite";

        private string TestDatabase
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

        [Fact]
        public void WithoutConnectionThrowsException()
        {
            var sqlite = new Sqlite("SQLite");
            sqlite.UpdateBaseUri(baseUri);

            Assert.Throws<DataStoreException>(() => sqlite.Build());
        }

        [Fact]
        public void SqliteGetsAName()
        {
            SQLiteConnection connection =
                new SQLiteConnection($"Data Source={TestDatabase}");
            
            var sqlite = new Sqlite("", baseUri, connection);
            
            sqlite.Build();

            Assert.Equal("SimpleDatabase", sqlite.Name);
        }

        [Fact]
        public void SqliteGetsSchemas()
        {
            SQLiteConnection connection =
                new SQLiteConnection($"Data Source={TestDatabase}");
            
            var sqlite = new Sqlite("", baseUri, connection);
            
            sqlite.Build();

            var expectedSqlite = new Sqlite("SimpleDatabase", baseUri);
            var expectedSchema = new Schema("main");
            expectedSqlite.AddStructure(expectedSchema);

            var actualSchema = sqlite.SubStructures.First();
            
            Assert.Equal(expectedSchema, actualSchema);
            Assert.IsType<Schema>(actualSchema);
        }

        [Fact]
        public void SqliteGetsTables()
        {
            SQLiteConnection connection =
                new SQLiteConnection($"Data Source={TestDatabase}");
            
            var sqlite = new Sqlite("", baseUri, connection);
            
            sqlite.Build();

            var expectedSqlite = new Sqlite("SimpleDatabase", baseUri);
            var expectedSchema = new Schema("main");
            var expectedTableUsers = new Table("Users");
            var expectedTableUserData = new Table("UserData");
            
            expectedSqlite.AddStructure(expectedSchema);
            expectedSchema.AddStructure(expectedTableUsers);
            expectedSchema.AddStructure(expectedTableUserData);

            var actualUsersTable = sqlite
                .SubStructures
                .First()
                .SubStructures
                .First(table => table.Name == "Users");

            var actualUserDataTable = sqlite
                .SubStructures
                .First()
                .SubStructures
                .First(table => table.Name == "UserData");

            Assert.Equal(expectedTableUsers, actualUsersTable);
            Assert.Equal(expectedTableUserData, actualUserDataTable);
            Assert.IsType<Table>(actualUsersTable);
            Assert.IsType<Table>(actualUserDataTable);
        }

        [Fact]
        public void SqliteGetsColumn()
        {
            SQLiteConnection connection =
                new SQLiteConnection($"Data Source={TestDatabase}");
            
            var sqlite = new Sqlite("", baseUri, connection);
            
            sqlite.Build();

            var expectedSqlite = new Sqlite("SimpleDatabase", baseUri);
            var expectedSchema = new Schema("main");
            var expectedTableUsers = new Table("Users");
            var expectedTableUserData = new Table("UserData");
            var expectedColumnEmail = new Column("email");
            var expectedColumnPhone = new Column("phone");
            
            expectedSqlite.AddStructure(expectedSchema);
            expectedSchema.AddStructure(expectedTableUsers);
            expectedSchema.AddStructure(expectedTableUserData);
            expectedTableUsers.AddStructure(expectedColumnEmail);
            expectedTableUserData.AddStructure(expectedColumnPhone);

            var actualColumnEmail = sqlite
                .SubStructures
                .First() 
                .SubStructures
                .First(table => table.Name == "Users") 
                .SubStructures
                .First(column => column.Name == "email"); 

            var actualColumnPhone = sqlite
                .SubStructures
                .First()
                .SubStructures
                .First(table => table.Name == "UserData")
                .SubStructures
                .First(column => column.Name == "phone");

            Assert.Equal(expectedColumnEmail, actualColumnEmail);
            Assert.Equal(expectedColumnPhone, actualColumnPhone);
            Assert.IsType<Column>(actualColumnEmail);
            Assert.IsType<Column>(actualColumnPhone);
        }

        [Fact]
        public void SqliteColumnsGetDataType()
        {
            // Test for email and phone
            SQLiteConnection connection =
                new SQLiteConnection($"Data Source={TestDatabase}");
            
            var sqlite = new Sqlite("", baseUri, connection);
            
            sqlite.Build();

            var expectedSqlite = new Sqlite("SimpleDatabase", baseUri);
            var expectedSchema = new Schema("main");
            var expectedTableUsers = new Table("Users");
            var expectedTableUserData = new Table("UserData");
            var expectedColumnEmail = new Column("email");
            var expectedColumnPhone = new Column("phone");
            
            expectedColumnEmail.SetDataType("VARCHAR");
            expectedColumnPhone.SetDataType("INT");
            
            expectedSqlite.AddStructure(expectedSchema);
            expectedSchema.AddStructure(expectedTableUsers);
            expectedSchema.AddStructure(expectedTableUserData);
            expectedTableUsers.AddStructure(expectedColumnEmail);
            expectedTableUserData.AddStructure(expectedColumnPhone);

            var actualColumnEmailDataType = sqlite
                .SubStructures
                .First() 
                .SubStructures
                .First(table => table.Name == "Users") 
                .SubStructures
                .Select(s => s as Column)
                .First(column => column?.Name == "email")
                ?.DataType; 

            var actualColumnPhoneDataType = sqlite
                .SubStructures
                .First()
                .SubStructures
                .First(table => table.Name == "UserData")
                .SubStructures
                .Select(s => s as Column)
                .First(column => column?.Name == "phone")
                ?.DataType;

            Assert.Equal(expectedColumnEmail.DataType, actualColumnEmailDataType);
            Assert.Equal(expectedColumnPhone.DataType, actualColumnPhoneDataType);
        }

        [Fact]
        public void SqliteTableGetsPrimaryKeys()
        {
            SQLiteConnection connection =
                new SQLiteConnection($"Data Source={TestDatabase}");
            
            var sqlite = new Sqlite("", baseUri, connection);
            
            sqlite.Build();

            var expectedSqlite = new Sqlite("SimpleDatabase", baseUri);
            var expectedSchema = new Schema("main");
            var expectedTableUsers = new Table("Users");
            var expectedTableUserData = new Table("UserData");
            var expectedColumnUsersId = new Column("id");
            var expectedColumnUserDataId = new Column("id");
            
            expectedSqlite.AddStructure(expectedSchema);
            expectedSchema.AddStructure(expectedTableUsers);
            expectedSchema.AddStructure(expectedTableUserData);
            expectedTableUsers.AddStructure(expectedColumnUsersId);
            expectedTableUserData.AddStructure(expectedColumnUserDataId);
            
            expectedTableUsers.AddPrimaryKey(expectedColumnUsersId);
            expectedTableUserData.AddPrimaryKey(expectedColumnUserDataId);
            
            
            var actualUsersPrimaryKey = sqlite
                .SubStructures
                .First()
                .SubStructures
                .Select(s => s as Table)
                .First(table => table?.Name == "Users")
                ?.PrimaryKeys
                .First();
            
            var actualUserDataPrimaryKey = sqlite
                .SubStructures
                .First()
                .SubStructures
                .Select(s => s as Table)
                .First(table => table?.Name == "UserData")
                ?.PrimaryKeys
                .First();

            

            Assert.Equal(expectedColumnUsersId, actualUsersPrimaryKey);
            Assert.Equal(expectedColumnUserDataId, actualUserDataPrimaryKey);
        }

        [Fact]
        public void SqliteTableGetsForeignKeys()
        {
            
        }

        [Fact]
        public void SqliteForeignKeyColumnsReferencesColumns()
        {
            
        }
    }
}