using System.Data.SQLite;
using System.IO;
using GraphManipulation.SchemaEvolution.Extensions;
using GraphManipulation.SchemaEvolution.Models;
using GraphManipulation.SchemaEvolution.Models.Stores;
using GraphManipulation.SchemaEvolution.Models.Structures;
using Xunit;

namespace Test;

public class SqliteTest
{
    private const string BaseUri = "http://www.test.com/";

    public class TestDatabaseFixture
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
    }

    public class NoForeignKeysDatabaseFixture
    {
        private static readonly string _testDatabase =
            $"TestResources{Path.DirectorySeparatorChar}NoForeignKeysDatabase.sqlite";

        private static readonly SQLiteConnection Connection = new($"Data Source={TestDatabase}");
        public readonly Column ExpectedColumnEmail = new("email");
        public readonly Column ExpectedColumnId = new("id");
        public readonly Column ExpectedColumnPassword = new("password");
        public readonly Schema ExpectedSchema = new("main");

        public readonly Sqlite ExpectedSqlite = new("NoForeignKeysDatabase", BaseUri);
        public readonly Table ExpectedTableUsers = new("Users");

        public readonly Sqlite Sqlite;


        public NoForeignKeysDatabaseFixture()
        {
            Sqlite = new Sqlite("", BaseUri, Connection);

            Sqlite.BuildFromDataSource();

            ExpectedSqlite.AddStructure(ExpectedSchema);

            ExpectedSchema.AddStructure(ExpectedTableUsers);

            ExpectedTableUsers.AddStructure(ExpectedColumnId);
            ExpectedTableUsers.AddStructure(ExpectedColumnEmail);
            ExpectedTableUsers.AddStructure(ExpectedColumnPassword);

            ExpectedTableUsers.AddPrimaryKey(ExpectedColumnId);

            ExpectedColumnId.SetDataType("INT");
            ExpectedColumnEmail.SetDataType("VARCHAR");
            ExpectedColumnPassword.SetDataType("VARCHAR");

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
    }

    public class NoPrimaryKeysDatabaseFixture
    {
        private static readonly string _testDatabase =
            $"TestResources{Path.DirectorySeparatorChar}NoPrimaryKeysDatabase.sqlite";

        private static readonly SQLiteConnection Connection = new($"Data Source={TestDatabase}");
        public readonly Column ExpectedColumnEmail = new("email");
        public readonly Column ExpectedColumnId = new("id");
        public readonly Column ExpectedColumnPassword = new("password");
        public readonly Schema ExpectedSchema = new("main");

        public readonly Sqlite ExpectedSqlite = new("NoPrimaryKeysDatabase", BaseUri);
        public readonly Table ExpectedTableUsers = new("Users");

        public readonly Sqlite Sqlite;


        public NoPrimaryKeysDatabaseFixture()
        {
            Sqlite = new Sqlite("", BaseUri, Connection);

            Sqlite.BuildFromDataSource();

            ExpectedSqlite.AddStructure(ExpectedSchema);

            ExpectedSchema.AddStructure(ExpectedTableUsers);

            ExpectedTableUsers.AddStructure(ExpectedColumnId);
            ExpectedTableUsers.AddStructure(ExpectedColumnEmail);
            ExpectedTableUsers.AddStructure(ExpectedColumnPassword);

            ExpectedColumnId.SetDataType("INT");
            ExpectedColumnEmail.SetDataType("VARCHAR");
            ExpectedColumnPassword.SetDataType("VARCHAR");

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

            Assert.Throws<DatabaseException>(() => sqlite.BuildFromDataSource());
        }

        [Fact]
        public void SqliteGetsAName()
        {
            Assert.Equal(_testDatabaseFixture.ExpectedSqlite.Name, _testDatabaseFixture.Sqlite.Name);
        }

        [Fact]
        public void SqliteGetsSchemas()
        {
            var actualSchema = _testDatabaseFixture.Sqlite.Find<Schema>(_testDatabaseFixture.ExpectedSchema);
            Assert.Equal(_testDatabaseFixture.ExpectedSchema, actualSchema);
        }

        [Fact]
        public void SqliteGetsTables()
        {
            var actualUsersTable = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUsers);

            var actualUserDataTable = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUserData);

            Assert.Equal(_testDatabaseFixture.ExpectedTableUsers, actualUsersTable);
            Assert.Equal(_testDatabaseFixture.ExpectedTableUserData, actualUserDataTable);
        }

        [Fact]
        public void SqliteGetsColumn()
        {
            var actualColumnEmail = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnEmail);

            var actualColumnPhone = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnPhone);

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail, actualColumnEmail);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone, actualColumnPhone);
        }

        [Fact]
        public void SqliteColumnsGetDataType()
        {
            var actualColumnEmailDataType = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnEmail)!
                .DataType;

            var actualColumnPhoneDataType = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnPhone)!
                .DataType;

            Assert.Equal(_testDatabaseFixture.ExpectedColumnEmail.DataType, actualColumnEmailDataType);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnPhone.DataType, actualColumnPhoneDataType);
        }

        [Fact]
        public void SqliteColumnsGetIsNotNull()
        {
            var actual = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnEmail)!
                .IsNotNull;

            Assert.True(actual);
        }

        [Fact]
        public void SqliteColumnsGetValidOptions()
        {
            var actual = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnUsersId)!
                .Options;

            var expected = "AUTOINCREMENT";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SqliteTableGetsPrimaryKeys()
        {
            var actualUsersPrimaryKey = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUsers)!
                .FindPrimaryKey("id");

            var actualUserDataPrimaryKey = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUserData)!
                .FindPrimaryKey("id");

            Assert.Equal(_testDatabaseFixture.ExpectedColumnUsersId, actualUsersPrimaryKey);
            Assert.Equal(_testDatabaseFixture.ExpectedColumnUserDataId, actualUserDataPrimaryKey);
        }

        [Fact]
        public void SqliteTableGetsForeignKeys()
        {
            var actualForeignKey = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUserData)!
                .FindForeignKey("id");

            Assert.Equal(_testDatabaseFixture.ExpectedForeignKeyUserDataIdUsersId, actualForeignKey);
        }

        [Fact]
        public void SqliteForeignKeyColumnReferenceColumn()
        {
            var actualReferencedColumn = _testDatabaseFixture.Sqlite
                .Find<Column>(_testDatabaseFixture.ExpectedColumnUsersId);

            Assert.Equal(_testDatabaseFixture.ExpectedForeignKeyUserDataIdUsersId.To, actualReferencedColumn);
        }

        [Fact]
        public void SqliteForeignKeysGetsOnDelete()
        {
            var actual = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUserData)!
                .FindForeignKey("id")!
                .OnDelete;

            Assert.Equal(ForeignKeyOnEnum.Cascade, actual);
        }

        [Fact]
        public void SqliteForeignKeysGetsOnUpdate()
        {
            var actual = _testDatabaseFixture.Sqlite
                .Find<Table>(_testDatabaseFixture.ExpectedTableUserData)!
                .FindForeignKey("id")!
                .OnUpdate;

            Assert.Equal(ForeignKeyOnEnum.NoAction, actual);
        }
    }

    public class NoForeignKeysBuildTest : IClassFixture<NoForeignKeysDatabaseFixture>
    {
        private readonly NoForeignKeysDatabaseFixture _testDatabaseFixture;

        public NoForeignKeysBuildTest(NoForeignKeysDatabaseFixture testDatabaseFixture)
        {
            _testDatabaseFixture = testDatabaseFixture;
        }

        [Fact]
        public void DatabaseWithoutForeignKeysWorks()
        {
            Assert.Equal(_testDatabaseFixture.ExpectedSqlite.Name, _testDatabaseFixture.Sqlite.Name);
        }
    }

    public class NoPrimaryKeysBuildTest : IClassFixture<NoPrimaryKeysDatabaseFixture>
    {
        private readonly NoPrimaryKeysDatabaseFixture _testDatabaseFixture;

        public NoPrimaryKeysBuildTest(NoPrimaryKeysDatabaseFixture testDatabaseFixture)
        {
            _testDatabaseFixture = testDatabaseFixture;
        }

        [Fact]
        public void DatabaseWithoutPrimaryKeysWorks()
        {
            Assert.Equal(_testDatabaseFixture.ExpectedSqlite.Name, _testDatabaseFixture.Sqlite.Name);
        }
    }
}