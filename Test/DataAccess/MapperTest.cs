using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using FluentAssertions;
using GraphManipulation.DataAccess;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using Xunit;

namespace Test.DataAccess;

public class MapperTest
{
    private class DomainEntityStub : DomainEntity
    {
    }

    public class Insert
    {
        [Fact]
        public void ThrowsArgumentNullExceptionIfArgumentIsNull()
        {
            Mapper<DomainEntityStub> mapper = new Mapper<DomainEntityStub>(null);
            Assert.Throws<ArgumentNullException>(() => mapper.Insert(null));
        }
    }

    public class Update
    {
        [Fact]
        public void ThrowsArgumentNullExceptionIfArgumentIsNull()
        {
            Mapper<DomainEntityStub> mapper = new Mapper<DomainEntityStub>(null);
            Assert.Throws<ArgumentNullException>(() => mapper.Update(null));
        }
    }

    public class Delete
    {
        [Fact]
        public void ThrowsArgumentNullExceptionIfArgumentIsNull()
        {
            Mapper<DomainEntityStub> mapper = new Mapper<DomainEntityStub>(null);
            Assert.Throws<ArgumentNullException>(() => mapper.Delete(null));
        }
    }

    public class PurposeExample : IDisposable
    {
        private static class SeedData
        {
            public static PersonalDataColumn Column1 = new()
            {
                Key = new TableColumnPair("tableOne", "columnOne")
            };

            public static PersonalDataColumn Column2 = new()
            {
                Key = new TableColumnPair("tableTwo", "columnTwo")
            };

            public static readonly VacuumingPolicy VacuumingPolicy1 = new()
                { Key = "policyOne", Duration = "2d", Description = "" };

            public static readonly VacuumingPolicy VacuumingPolicy2 = new()
                { Key = "policyTwo", Duration = "3d", Description = "" };

            public static Purpose purpose1 = new()
            {
                Key = "purposeOne",
                VacuumingPolicies = new[] { VacuumingPolicy1 }
            };

            public static Purpose purpose2 = new()
            {
                Key = "purposeTwo",
                VacuumingPolicies = new[] { VacuumingPolicy1, VacuumingPolicy2 }
            };

            public static void SeedDatabase(GdprMetadataContext context)
            {
                context.purposes.Add(purpose1);
                context.purposes.Add(purpose2);
                context.SaveChanges();
            }
        }

        // Mapper is generic, and requires an instance of DbContext to test.
        // To get any value from testing, the DbContext should be connected to an actual database.
        // To reduce the number of duplicated tests, only Mapper<Purpose> is tested with a database
        private readonly GdprMetadataContext _context;
        private const string DatabaseFile = "MapperTest.sqlite";

        public PurposeExample()
        {
            // Create the database
            SQLiteConnection.CreateFile(DatabaseFile);
            _context = new GdprMetadataContext($"Data Source={DatabaseFile}");
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public void InsertAddsId()
        {
            // Insert is tested further in other tests
            // Assignment of id is the only functionality that can be tested in isolation.
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            var testPurpose = new Purpose() { Key = "TestPurpose" };

            var insertedPurpose = mapper.Insert(testPurpose);

            insertedPurpose.Id.Should().NotBeNull();
        }

        [Fact]
        public void FindReturnsEmptyListWhenNoMatches()
        {
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            IEnumerable<Purpose> result = mapper.Find(purpose => purpose.Key == "NoSuchPurpose");

            result.Should().BeEmpty();
        }

        [Fact]
        public void FindReturnsMatchingElements()
        {
            SeedData.SeedDatabase(_context);
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            IEnumerable<Purpose> result = mapper.Find(purpose => purpose.Key.Contains("purpose"));

            result.Should().Contain(SeedData.purpose1);
            result.Should().Contain(SeedData.purpose2);
        }

        [Fact]
        public void FindSingleReturnsNullWhenNoMatch()
        {
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            Purpose? result = mapper.FindSingle(purpose => purpose.Id == 1234);

            result.Should().BeNull();
        }

        [Fact]
        public void FindSingleReturnsOnlyMatchingElement()
        {
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            var expectedPurpose = new Purpose() { Key = "TestPurpose" };
            mapper.Insert(expectedPurpose);

            Purpose? actualPurpose = mapper.FindSingle(purpose => purpose.Key == "TestPurpose");

            actualPurpose.Should().Be(expectedPurpose);
        }

        [Fact]
        public void FindSingleTableColumnFromPersonalDataColumn()
        {
            Mapper<PersonalDataColumn> mapper = new Mapper<PersonalDataColumn>(_context);
            var expectedColumn = new PersonalDataColumn
            {
                DefaultValue = "",
                Description = "",
                Key = new TableColumnPair("Table", "Column")
            };
            mapper.Insert(expectedColumn);

            PersonalDataColumn? result = mapper.FindSingle(x =>
                x.Key.Equals(expectedColumn.Key));
            var tableColumnPair = result?.Key;

            tableColumnPair.Should().Be(expectedColumn.Key);
        }

        [Fact]
        public void FindSingleThrowsExceptionWhenMultipleMatches()
        {
            SeedData.SeedDatabase(_context);
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            Assert.Throws<InvalidOperationException>(() =>
                mapper.FindSingle(purpose => purpose.Key.Contains("purpose")));
        }

        [Fact]
        public void UpdateInsertsGivenNonExistingElement()
        {
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            var newPurpose = new Purpose() { Key = "NoSuchPurpose" };

            mapper.Update(newPurpose);
            var fetchedPurpose = mapper.FindSingle(purpose => purpose.Key == "NoSuchPurpose");

            fetchedPurpose.Should().Be(newPurpose);
        }

        [Fact]
        public void UpdatesExistingValue()
        {
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);

            var newPurpose = new Purpose() { Key = "OriginalValue" };
            mapper.Insert(newPurpose);

            newPurpose.Key = "NewValue";
            mapper.Update(newPurpose);

            var updatedPurpose = mapper.FindSingle(purpose => purpose.Id == newPurpose.Id);
            updatedPurpose.Key.Should().Be("NewValue");
        }

        [Fact]
        public void DeletingNonExistingEntryThrowsInvalidOperationException()
        {
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);
            var nonExistingPurpose = new Purpose() { Key = "NoSuchPurpose" };

            Assert.Throws<InvalidOperationException>(() => mapper.Delete(nonExistingPurpose));
        }

        [Fact]
        public void DeleteRemovesGivenValue()
        {
            SeedData.SeedDatabase(_context);
            Mapper<Purpose> mapper = new Mapper<Purpose>(_context);

            mapper.Delete(SeedData.purpose2);

            var allPurposes = mapper.Find(purpose => true);
            allPurposes.Should().Contain(SeedData.purpose1);
            allPurposes.Should().NotContain(SeedData.purpose2);
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