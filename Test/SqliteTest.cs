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
            
            Assert.Equal(expectedSchema, sqlite.SubStructures.First());
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
            
            Assert.Equal(expectedTableUsers, sqlite.SubStructures.First().SubStructures.First(s => s.Name == "Users"));
            Assert.Equal(expectedTableUserData, sqlite.SubStructures.First().SubStructures.First(s => s.Name == "UserData"));
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
            var expectedColumnEmail = new Table("email");
            var expectedColumnAddress = new Table("address");
            
            expectedSqlite.AddStructure(expectedSchema);
            expectedSchema.AddStructure(expectedTableUsers);
            expectedSchema.AddStructure(expectedTableUserData);
            expectedTableUsers.AddStructure(expectedColumnEmail);
            expectedTableUserData.AddStructure(expectedColumnAddress);
            
            Assert.Equal(expectedColumnEmail, 
                sqlite.SubStructures
                    .First()
                    .SubStructures
                    .First(s => s.Name == "Users")
                    .SubStructures
                    .First(s => s.Name == "email"));
        }
    }
}