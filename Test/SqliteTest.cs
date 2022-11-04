using System;
using System.Data.SQLite;
using System.IO;
using GraphManipulation.Extensions;
using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using Xunit;

namespace Test;

public class SqliteTest
{
    private const string BaseUri = "http://www.test.com/";

    public class TestDatabaseFixture : IDisposable
    {
        private static readonly string _testDatabase =
            $"TestResources{Path.DirectorySeparatorChar}SimpleDatabase.sqlite";

        private static readonly SQLiteConnection Connection = new($"Data Source={TestDatabase}");
        public readonly Column ExpectedColumnEmail = new("email");
        public readonly Column ExpectedColumnPhone = new("phone");
        public readonly Column ExpectedColumnUserDataId = new("id");
        public readonly Column ExpectedColumnUsersId = new("id");
        public readonly ForeignKey ExpectedForeignKeyUserDataIdUsersId;
        public readonly Schema ExpectedSchema = new("main");

        public readonly Sqlite ExpectedSqlite = new("SimpleDatabase", BaseUri);
        public readonly Table ExpectedTableUserData = new("UserData");
        public readonly Table ExpectedTableUsers = new("Users");

        public readonly Sqlite Sqlite;


        public TestDatabaseFixture()
        {
            Sqlite = new Sqlite("", BaseUri, Connection);

            Sqlite.BuildFromDataSource();

            ExpectedSqlite.AddStructure(ExpectedSchema);

            ExpectedSchema.AddStructure(ExpectedTableUsers);
            ExpectedSchema.AddStructure(ExpectedTableUserData);

            ExpectedTableUsers.AddStructure(ExpectedColumnUsersId);
            ExpectedTableUsers.AddStructure(ExpectedColumnEmail);

            ExpectedTableUserData.AddStructure(ExpectedColumnUserDataId);
            ExpectedTableUserData.AddStructure(ExpectedColumnPhone);

            ExpectedTableUsers.AddPrimaryKey(ExpectedColumnUsersId);
            ExpectedTableUserData.AddPrimaryKey(ExpectedColumnUserDataId);

            ExpectedForeignKeyUserDataIdUsersId = new ForeignKey(ExpectedColumnUserDataId, ExpectedColumnUsersId,
                ForeignKeyOnEnum.Cascade);

            ExpectedTableUserData.AddForeignKey(ExpectedForeignKeyUserDataIdUsersId);

            ExpectedColumnEmail.SetDataType("VARCHAR");
            ExpectedColumnPhone.SetDataType("INT");

            ExpectedColumnEmail.SetIsNotNull(true);
        }

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

            Assert.Throws<DataStoreException>(() => sqlite.BuildFromDataSource());
        }

        [Fact]
        public void SqliteGetsAName()
        {
            Assert.Equal("SimpleDatabase", _testDatabaseFixture.Sqlite.Name);
        }

        [Fact]
        public void SqliteGetsSchemas()
        {
            var actualSchema = _testDatabaseFixture.Sqlite.FindSchema("main");
            Assert.Equal(_testDatabaseFixture.ExpectedSchema, actualSchema);
        }

        [Fact]
        public void SqliteGetsTables()
        {
            var actualUsersTable = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("Users");

            var actualUserDataTable = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData");

            Assert.Equal(_testDatabaseFixture.ExpectedTableUsers, actualUsersTable);
            Assert.Equal(_testDatabaseFixture.ExpectedTableUserData, actualUserDataTable);
        }

        [Fact]
        public void SqliteGetsColumn()
        {
            var actualColumnEmail =
                _testDatabaseFixture.Sqlite
                    .FindSchema("main")
                    .FindTable("Users")
                    .FindColumn("email");

            var actualColumnPhone = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData")
                .FindColumn("phone");

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail, actualColumnEmail);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone, actualColumnPhone);
        }

        [Fact]
        public void SqliteColumnsGetDataType()
        {
            var actualColumnEmailDataType = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("Users")
                .FindColumn("email")
                .DataType;

            var actualColumnPhoneDataType = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData")
                .FindColumn("phone")
                .DataType;

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail.DataType, actualColumnEmailDataType);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone.DataType, actualColumnPhoneDataType);
        }

        [Fact]
        public void SqliteColumnsGetIsNotNull()
        {
            var actual = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("Users")
                .FindColumn("email")
                .IsNotNull;

            Assert.True(actual);
        }

        [Fact]
        public void SqliteColumnsGetValidOptions()
        {
            var actual = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("Users")
                .FindColumn("id")
                .Options;

            var expected = "AUTOINCREMENT";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SqliteTableGetsPrimaryKeys()
        {
            var actualUsersPrimaryKey = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("Users")
                .FindPrimaryKey("id");

            var actualUserDataPrimaryKey = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData")
                .FindPrimaryKey("id");

            Assert.Equal(_testDatabaseFixture.ExpectedColumnUsersId, actualUsersPrimaryKey);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnUserDataId, actualUserDataPrimaryKey);
        }

        [Fact]
        public void SqliteTableGetsForeignKeys()
        {
            var actualForeignKey = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData")
                .FindForeignKey("id");

            Assert.Equal(_testDatabaseFixture.ExpectedForeignKeyUserDataIdUsersId, actualForeignKey);
        }

        [Fact]
        public void SqliteForeignKeyColumnReferenceColumn()
        {
            var actualReferencedColumn = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("Users")
                .FindColumn("id");

            Assert.Equal(_testDatabaseFixture.ExpectedForeignKeyUserDataIdUsersId.To, actualReferencedColumn);
        }

        [Fact]
        public void SqliteForeignKeysGetsOnDelete()
        {
            var actual = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData")
                .FindForeignKey("id")
                .OnDelete;

            Assert.Equal(ForeignKeyOnEnum.Cascade, actual);
        }

        [Fact]
        public void SqliteForeignKeysGetsOnUpdate()
        {
            var actual = _testDatabaseFixture.Sqlite
                .FindSchema("main")
                .FindTable("UserData")
                .FindForeignKey("id")
                .OnUpdate;

            Assert.Equal(ForeignKeyOnEnum.NoAction, actual);
        }
    }

    public class CreateStatementTest : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _testDatabaseFixture;

        public CreateStatementTest(TestDatabaseFixture testDatabaseFixture)
        {
            _testDatabaseFixture = testDatabaseFixture;
        }

        public class To
        {
            [Fact]
            public void BaseTest()
            {
            }


            [Fact]
            public void FullTest()
            {
            }
        }

        public class From
        {
        }
    }
}