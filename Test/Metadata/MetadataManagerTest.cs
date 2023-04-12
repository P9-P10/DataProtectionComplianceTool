using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Dapper;
using FluentAssertions;
using GraphManipulation.MetadataManagement;
using Xunit;

namespace Test.Metadata;

public class MetadataManagerTest : IDisposable
{
    public MetadataManagerTest()
    {
        // Define the name of the table containing individuals
        IndividualsTable = "individuals";
        // Create an in-memory database
        SQLiteConnection.CreateFile("TestDatabase.sqlite");
        var connectionString = "Data Source=TestDatabase.sqlite";

        Manager = new MetadataManager(connectionString, IndividualsTable);
        Connection = Manager.Connection;

        // Create a table for individuals and provide some seed data
        var createIndividualsTable = @$"
            DROP TABLE IF EXISTS individuals;
            CREATE TABLE {IndividualsTable}(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name VARCHAR
            );
            INSERT INTO {IndividualsTable}(name) VALUES
	        ('Alice'), ('Bob'), ('Charlie');
        ";

        // Execute the statement
        Connection.Execute(createIndividualsTable, new { IndividualsTable });
        Connection.Close();
    }

    private IDbConnection Connection { get; }
    private string IndividualsTable { get; }
    
    private MetadataManager Manager { get; }

    public void Dispose()
    {
        // Dispose of the connection.
        // This will delete the in-memory database.
        Connection.Close();
        Connection.Dispose();
        Manager.Dispose();
        File.Delete("TestDatabase.sqlite");
    }

    [Fact]
    public void CreatesAndRemovesTables()
    {
        // Database should initially contain only the individuals table
        Assert.Equal(new List<string> { IndividualsTable }, getTablesInDatabase());

        // Create metadata tables
        Manager.CreateMetadataTables();

        // Database should now contain metadata tables
        getTablesInDatabase().Should().Contain(new List<string>
        {
            IndividualsTable,
            "metadata_columns",
            "gdpr_metadata",
            "delete_conditions",
            "user_metadata",
            "personal_data_processing"
        });
        
        // Remove the metadata tables
        Manager.DropMetadataTables();

        // Database should again contain only the individuals table
        Assert.Equal(new List<string> { IndividualsTable }, getTablesInDatabase());
    }

    private IEnumerable<string> getTablesInDatabase()
    {
        // Return a list of all non-system tables
        // Query taken from https://www.sqlitetutorial.net/sqlite-show-tables/
        return Connection.Query<string>(@"
            SELECT 
                name
                    FROM 
                sqlite_schema
                WHERE 
                    type ='table' AND 
                    name NOT LIKE 'sqlite_%';
        ");
    }

    [Fact]
    public void AddsMetadataAndReferences()
    {
        Manager.CreateMetadataTables();

        Manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));

        var insertedRow =
            Connection.QuerySingle<(int, int)>("select id, target_column from gdpr_metadata");

        // Check that the expected values were inserted into gdpr_metadata
        Assert.Equal((1, 1), insertedRow);

        var targetColumn =
            Connection.QuerySingle<(int, string, string)>(
                "select id, target_table, target_column from metadata_columns");
        
        Assert.Equal((1, "mockTable", "mockColumn"), targetColumn);

        var insertedReferences = Connection.Query<(int, int)>("select user_id, metadata_id from user_metadata");

        // Assert that a reference between each individual and the metadata was inserted.
        Assert.Contains((1, 1), insertedReferences);
        Assert.Contains((2, 1), insertedReferences);
        Assert.Contains((3, 1), insertedReferences);
    }
    
    [Fact]
    public void ThrowsExceptionIfTargetTableAndColumnNotDefined()
    {
        Manager.CreateMetadataTables();
        
        Assert.Throws<ArgumentException>(() => Manager.MarkAsPersonalData(new GDPRMetadata()));
    }

    [Fact]
    public void AddsAllDefinedMetadataValues()
    {
        Manager.CreateMetadataTables();

        var expected = new GDPRMetadata("mockTable", "mockColumn") { Purpose = "Testing", Origin = "Imagination" };
        Manager.MarkAsPersonalData(expected);

        var actual = Manager.GetMetadataEntry(1);

        // Check that the expected values were inserted into gdpr_metadata
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddsPurpose()
    {
        Manager.CreateMetadataTables();
        Manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));

        Manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn") { Purpose = "Testing!" });

        var purpose = Connection.QuerySingle<string>("select purpose from gdpr_metadata");

        Assert.Equal("Testing!", purpose);
    }

    [Fact]
    public void AddsOrigin()
    {
        Manager.CreateMetadataTables();
        Manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));

        Manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn") { Origin = "Imagination" });

        var origin = Connection.QuerySingle<string>("select origin from gdpr_metadata");

        Assert.Equal("Imagination", origin);
    }

    [Fact]
    public void AddsLegallyRequired()
    {
        Manager.CreateMetadataTables();
        Manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));

        Manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn") { LegallyRequired = true });

        var legallyRequired = Connection.QuerySingle<bool>("select legally_required from gdpr_metadata where id = 1");

        Assert.Equal(true, legallyRequired);
    }

    [Fact]
    public void UpdateIgnoresNullValues()
    {
        Manager.CreateMetadataTables();
        Manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn") { Purpose = "Test" });

        Manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn") { LegallyRequired = true });

        var purpose = Connection.QuerySingle<string>("select purpose from gdpr_metadata");
        var legallyRequired = Connection.QuerySingle<bool>("select legally_required from gdpr_metadata where id = 1");

        Assert.Equal("Test", purpose);
        Assert.Equal(true, legallyRequired);
    }

    [Fact]
    public void GetsMetadataWithMissingValues()
    {
        Manager.CreateMetadataTables();

        // Defines only necessary values
        var one = new GDPRMetadata("mockTable", "mockColumn");
        // Defines all values
        var two = new GDPRMetadata("mockTable", "mockColumn")
        {
            Purpose = "purpose",
            LegallyRequired = true,
            Origin = "test",
        };
        // Defines all values except 'origin'
        var three = new GDPRMetadata("mockTable", "mockColumn")
        {
            Purpose = "purpose",
            LegallyRequired = false,
        };

        Manager.MarkAsPersonalData(one);
        Manager.MarkAsPersonalData(two);
        Manager.MarkAsPersonalData(three);

        var result = Manager.GetMetadataWithNullValues();

        Assert.Contains(one, result);
        Assert.DoesNotContain(two, result);
        Assert.Contains(three, result);
    }
}