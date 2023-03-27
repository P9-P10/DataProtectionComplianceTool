using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Dapper;
using GraphManipulation.Components;
using GraphManipulation.Models.Metadata;
using Xunit;

namespace Test;

public class MetadataManagerTest : IDisposable
{
    private IDbConnection Connection { get; }
    private string IndividualsTable { get; }

    public MetadataManagerTest()
    {
        // Define the name of the table containing individuals
        IndividualsTable = "individuals";
        // Create an in-memory database
        SQLiteConnection.CreateFile("TestDatabase.sqlite");
        string connectionString = "Data Source=TestDatabase.sqlite";
        Connection = new SQLiteConnection(connectionString);

        // Create a table for individuals and provide some seed data
        string createIndividualsTable = @$"
            DROP TABLE IF EXISTS individuals;
            CREATE TABLE {IndividualsTable}(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name VARCHAR
            );
            INSERT INTO {IndividualsTable}(name) VALUES
	        ('Alice'), ('Bob'), ('Charlie');
        ";

        // Execute the statement
        Connection.Execute(createIndividualsTable, new { IndividualsTable = IndividualsTable });
    }
    
    public void Dispose()
    {
        // Dispose of the connection.
        // This will delete the in-memory database.
        Connection.Close();
        Connection.Dispose();
        File.Delete("TestDatabase.sqlite");
    }

    [Fact]
    public void CreatesAndRemovesTables()
    {
        // Database should initially contain only the individuals table
        Assert.Equal(new List<string>{ IndividualsTable }, getTablesInDatabase());
        
        // Create metadata tables
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        
        // Database should now contain metadata tables
        Assert.Equal(
            new List<string>{ IndividualsTable, "gdpr_metadata", "user_metadata", "personal_data_processing" },
            getTablesInDatabase()
            );
        
        // Remove the metadata tables
        manager.DropMetadataTables();
        
        // Database should again contain only the individuals table
        Assert.Equal(new List<string>{ IndividualsTable }, getTablesInDatabase());
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
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));

        (int, string, string) insertedRow = Connection.QuerySingle<(int, string, string)>("select id, target_table, target_column from gdpr_metadata");
        
        // Check that the expected values were inserted into gdpr_metadata
        Assert.Equal((1, "mockTable", "mockColumn"), insertedRow);
        
        IEnumerable<(int, int)> insertedReferences = Connection.Query<(int, int)>("select user_id, metadata_id from user_metadata");

        // Assert that a reference between each individual and the metadata was inserted.
        Assert.Contains((1, 1), insertedReferences);
        Assert.Contains((2, 1), insertedReferences);
        Assert.Contains((3, 1), insertedReferences);
    }
    
    [Fact]
    public void AddsAllDefinedMetadataValues()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();

        GDPRMetadata expected = new GDPRMetadata("mockTable", "mockColumn") { Purpose = "Testing", TTL = "today" };
        manager.MarkAsPersonalData(expected);

        GDPRMetadata actual = manager.GetMetadataEntry(1);
        
        // Check that the expected values were inserted into gdpr_metadata
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AddsPurpose()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));
        
        manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn"){Purpose = "Testing!"});
        
        string purpose = Connection.QuerySingle<string>("select purpose from gdpr_metadata where id = 1");
        
        Assert.Equal("Testing!", purpose);
    }
    
    [Fact]
    public void AddsTTL()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));
        
        manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn"){TTL = "one"});
        
        string ttl = Connection.QuerySingle<string>("select ttl from gdpr_metadata where id = 1");
        
        Assert.Equal("one", ttl);
    }
    
    [Fact]
    public void AddsOrigin()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));
        
        manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn"){Origin = "Imagination"});
        
        string origin = Connection.QuerySingle<string>("select origin from gdpr_metadata where id = 1");
        
        Assert.Equal("Imagination", origin);
    }

    [Fact]
    public void AddsStartTime()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));
        
        manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn"){StartTime = "Yesterday"});
        
        string startTime = Connection.QuerySingle<string>("select start_time from gdpr_metadata where id = 1");
        
        Assert.Equal("Yesterday", startTime);
    }

    [Fact]
    public void AddsLegallyRequired()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn"));
        
        manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn") {LegallyRequired = true});
        
        bool legallyRequired = Connection.QuerySingle<bool>("select legally_required from gdpr_metadata where id = 1");
        
        Assert.Equal(true, legallyRequired);
    }
    
    [Fact]
    public void UpdateIgnoresNullValues()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();
        manager.MarkAsPersonalData(new GDPRMetadata("mockTable", "mockColumn") {Purpose = "Test"});
        
        manager.UpdateMetadataEntry(1, new GDPRMetadata("mockTable", "mockColumn") {LegallyRequired = true});
        
        string purpose = Connection.QuerySingle<string>("select purpose from gdpr_metadata where id = 1");
        bool legallyRequired = Connection.QuerySingle<bool>("select legally_required from gdpr_metadata where id = 1");
        
        Assert.Equal("Test", purpose);
        Assert.Equal(true, legallyRequired);
    }
    
    [Fact]
    public void GetsMetadataWithMissingValues()
    {
        MetadataManager manager = new MetadataManager(Connection, IndividualsTable);
        manager.CreateMetadataTables();

        // Defines only necessary values
        GDPRMetadata one = new GDPRMetadata("mockTable", "mockColumn");
        // Defines all values
        GDPRMetadata two = new GDPRMetadata("mockTable", "mockColumn")
        {
            Purpose = "purpose",
            LegallyRequired = false,
            Origin = "test",
            StartTime = "now",
            TTL = "3 days"
        };
        // Defines all values except 'origin'
        GDPRMetadata three = new GDPRMetadata("mockTable", "mockColumn")
        {
            Purpose = "purpose",
            LegallyRequired = false,
            StartTime = "now",
            TTL = "3 days"
        };

        manager.MarkAsPersonalData(one);
        manager.MarkAsPersonalData(two);
        manager.MarkAsPersonalData(three);

        IEnumerable<GDPRMetadata> result = manager.GetMetadataWithNullValues();

        Assert.Contains(one, result);
        Assert.DoesNotContain(two, result);
        Assert.Contains(three, result);
    }
}