using System;
using System.Data.SQLite;
using System.IO;
using Dapper;
using FluentAssertions;
using GraphManipulation.DataAccess;
using Xunit;

namespace Test.DataAccess;

public class DatabaseInteractionTest
{
    public class CodeFirstDatabaseCreation : IDisposable
    {
        private readonly GdprMetadataContext _context;
        private const string DatabaseFile = "EfCoreTest.sqlite";

        public CodeFirstDatabaseCreation()
        {
            SQLiteConnection.CreateFile(DatabaseFile);
            _context = new GdprMetadataContext($"Data Source={DatabaseFile}");
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
        
        [Fact]
        public void CreatesTablesForEntities()
        {
            var result = _context.Connection.Query<string>(@"
            SELECT 
                name
                    FROM 
                sqlite_schema
                WHERE 
                    type ='table' AND 
                    name NOT LIKE 'sqlite_%';
            ");
            
            result.Should().BeEquivalentTo( new []{
                "purposes", 
                "origins", 
                "columns", 
                "vacuumingRules",
                "processes",
                "people",
                "PurposeVacuumingRule",
                "PersonalDataColumnPurpose",
                "DeleteCondition",
                "personalDatas"
            });
        }
        
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Connection.Close();
            _context.Connection.Dispose();
            _context.Dispose();
            File.Delete(DatabaseFile);
        }
    }
}