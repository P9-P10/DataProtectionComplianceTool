﻿using System.Collections;
using System.Data;
using Dapper;
using GraphManipulation.Helpers;
using GraphManipulation.MetadataManagement.AttributeMapping;
using GraphManipulation.Models.Metadata;

namespace GraphManipulation.Components;

public class MetadataManager
{
    private IDbConnection _connection;
    private string _individualsTable;
    /// <summary>
    /// Provides methods for interacting with GDPR metadata stored in a database represented by the given IDbConnection.
    /// </summary>
    /// <param name="connection">Connection to database containing metadata</param>
    /// <param name="tableContainingIndividuals">Table containing individuals. Must have id column.</param>
    public MetadataManager(IDbConnection connection, string tableContainingIndividuals)
    {
        _connection = connection;
        _individualsTable = tableContainingIndividuals;
        
        // Add custom mapper that uses attributes to map columns to properties to dapper.
        SqlMapper.SetTypeMap(
            typeof(GDPRMetadata),
            new ColumnAttributeTypeMapper<GDPRMetadata>());
        
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

    public void MarkAsPersonalData(GDPRMetadata metadata)
    {
        InsertStatementBuilder builder = new InsertStatementBuilder("gdpr_metadata");
        builder.InsertValues = metadata;
        
        // This sql statement inserts a row with the given values, and returns the id of the inserted row
        // Please note that the function 'last_insert_rowid()' is specific to sqlite.
        string insertMetadataStatement = builder.Build() + " select last_insert_rowid();";
        int metadataId = _connection.QuerySingle<int>(insertMetadataStatement);

        string addReferencesToMetadataStatement = $@"
          INSERT INTO user_metadata (user_id, metadata_id, objection, automated_decisionmaking) 
	        SELECT id, {metadataId}, false, false from {_individualsTable}
        ";
        
        _connection.Execute(addReferencesToMetadataStatement);
    }

    public void UpdateMetadataEntry(int entryId, GDPRMetadata value)
    {
        IEnumerable<(string, object)> columns = typeof(GDPRMetadata).GetProperties().Where(prop => prop.GetValue(value) != null).Select(prop =>
            (prop.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault().Name, prop.GetValue(value)));

        IEnumerable<string> valueUpdates = columns
            .Select((col) => col.Item2 is string ? $"{col.Item1} = '{col.Item2}'" : $"{col.Item1} = {col.Item2.ToString().ToLower()}");
        
        string setValues = string.Join(", ", valueUpdates);
        string updateStatement = $@"
        UPDATE gdpr_metadata 
        SET {setValues}
        WHERE id = {entryId}
        ";

        _connection.Execute(updateStatement);
    }

    public GDPRMetadata GetMetadataEntry(int entryId)
    {
        GDPRMetadata result = _connection.QuerySingle<GDPRMetadata>(
            $"select target_table, target_column, purpose, ttl, origin, start_time, legally_required from gdpr_metadata where id = {entryId}");

        return result;
    }
    
    public IEnumerable<GDPRMetadata> GetMetadataWithNullValues()
    {
        IEnumerable<string> columns = typeof(GDPRMetadata).GetProperties().Select(prop =>
            prop.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault().Name);
        
        IEnumerable<GDPRMetadata> result = _connection.Query<GDPRMetadata>(@$"
            select {string.Join(", ", columns)} from gdpr_metadata 
            where {string.Join(" IS NULL OR ", columns)} IS NULL");

        return result;
    }
}