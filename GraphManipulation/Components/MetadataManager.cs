using System.Data;
using System.Data.SQLite;
using Dapper;

namespace GraphManipulation.Components;

public class MetadataManager
{
    private IDbConnection _connection;
    private string _individualsTable;
    /// <summary>
    /// Provides methods for interacting with GDPR metadata stored in a database represented by the given IDbConnection.
    /// </summary>
    /// <param name="connection">Connection to database containing metadata</param>
    /// <param name="tableContainingIndivduals">Table containing individuals. Must have id column.</param>
    public MetadataManager(IDbConnection connection, string tableContainingIndivduals)
    {
        _connection = connection;
        _individualsTable = tableContainingIndivduals;
    }

    /// <summary>
    /// Creates the tables needed to store GDPR metadata.
    /// </summary>
    public void CreateMetadataTables()
    {
        string createMetadataStatement = @$"
        CREATE TABLE gdpr_metadata
        (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            purpose VARCHAR,
            ttl VARCHAR,
            target_table VARCHAR,
            target_column VARCHAR,
            origin VARCHAR,
            start_time VARCHAR,
            legally_required INTEGER
        );

        CREATE TABLE user_metadata(
            user_id INTEGER,
            metadata_id INTEGER,
            objection INTEGER,
            automated_decisionmaking INTEGER,
            PRIMARY KEY (user_id, metadata_id),
            FOREIGN KEY (user_id) REFERENCES {_individualsTable}(id),
            FOREIGN KEY (metadata_id) REFERENCES gdpr_metadata(id)
        );

        CREATE TABLE personal_data_processing(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            metadata_id INTEGER,
            process VARCHAR,
            FOREIGN KEY (metadata_id) REFERENCES gdpr_metadata(id)
        );
    ";
        
        _connection.Execute(createMetadataStatement);
    }

    public void DropMetadataTables()
    {
        string dropMetadataStatement = @"
        DROP TABLE IF EXISTS gdpr_metadata;
        DROP TABLE IF EXISTS user_metadata;
        DROP TABLE IF EXISTS personal_data_processing;
        ";
        
        _connection.Execute(dropMetadataStatement);
    }

    public void MarkAsPersonalData(string table, string column)
    {
        // This sql statement inserts a row with the given values, and returns the id of the inserted row
        // Please note that the function 'last_insert_rowid()' is specific to sqlite.
        string insertMetadataStatement = @$"
          INSERT INTO gdpr_metadata (target_table, target_column)
	         VALUES ('{table}', '{column}');
            select last_insert_rowid();
        ";
        int metadataId = _connection.QuerySingle<int>(insertMetadataStatement);

        string addReferencesToMetadataStatement = $@"
          INSERT INTO user_metadata (user_id, metadata_id, objection, automated_decisionmaking) 
	        SELECT id, {metadataId}, false, false from {_individualsTable}
        ";
        
        _connection.Execute(addReferencesToMetadataStatement);
    }

    public void AddPurpose()
    {
    }

    public void AddTTL()
    {
    }

    public void AddOrigin()
    {
    }
}