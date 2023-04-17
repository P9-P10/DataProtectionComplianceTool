using System.Data;
using System.Data.Entity;
using Dapper;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Entities;
using Microsoft.Data.Sqlite;

namespace GraphManipulation.MetadataManagement;
// TODO: Vi skal have en måde at hive alle nuværende rækker i gdpr_metadata ud, så brugeren kan se hvilke entryID'er de har

// TODO: Vi skal have en måde at hive delete conditions ud, så vi kan logge hvilke conditions der er blevet håndhævet

public class MetadataManager : IMetadataManager, IDisposable
{
    public readonly IDbConnection Connection;
    private readonly string _individualsTable;
    private readonly MetadataDbContext _context;

    /// <summary>
    ///     Provides methods for interacting with GDPR metadata stored in a database represented by the given IDbConnection.
    /// </summary>
    /// <param name="connection">Connection to database containing metadata</param>
    /// <param name="tableContainingIndividuals">Table containing individuals. Must have id column.</param>
    public MetadataManager(string connectionString, string tableContainingIndividuals)
    {
        _individualsTable = tableContainingIndividuals;
        _context = new MetadataDbContext(connectionString);
        Connection = _context.Connection;
    }

    /// <summary>
    ///     Creates the tables needed to store GDPR metadata.
    /// </summary>
    public void CreateMetadataTables()
    {
        var createMetadataStatement = @$"
       CREATE TABLE metadata_columns(
	         id INTEGER PRIMARY KEY AUTOINCREMENT,
	         target_table varchar NOT NULL,
	         target_column varchar NOT NULL,
	         default_value varchar
	         );
        
        CREATE TABLE gdpr_metadata(
               id INTEGER PRIMARY KEY AUTOINCREMENT,
               purpose VARCHAR,
               target_column INTEGER NOT NULL,
               origin VARCHAR,
               legally_required INTEGER,
               FOREIGN KEY (target_column) REFERENCES metadata_columns(id)
               );
        
        CREATE TABLE delete_conditions (
               id INTEGER PRIMARY KEY AUTOINCREMENT,
               metadata_id INTEGER,
               condition VARCHAR,
               FOREIGN KEY (metadata_id) REFERENCES gdpr_metadata(id) ON DELETE CASCADE
               );

        CREATE TABLE user_metadata(
             user_id INTEGER,
             metadata_id INTEGER,
             objection INTEGER,
             automated_decisionmaking INTEGER,
             PRIMARY KEY (user_id, metadata_id),
             FOREIGN KEY (user_id) REFERENCES {_individualsTable}(id),
             FOREIGN KEY (metadata_id) REFERENCES gdpr_metadata(id) ON DELETE CASCADE
             );

        CREATE TABLE personal_data_processing(
             id INTEGER PRIMARY KEY AUTOINCREMENT,
             metadata_id INTEGER,
             process VARCHAR,
             FOREIGN KEY (metadata_id) REFERENCES gdpr_metadata(id) ON DELETE CASCADE
             );
    ";

        Connection.Execute(createMetadataStatement);
    }

    public void DropMetadataTables()
    {
        var dropMetadataStatement = @"
        DROP TABLE IF EXISTS metadata_columns;
        DROP TABLE IF EXISTS gdpr_metadata;
        DROP TABLE IF EXISTS delete_conditions;
        DROP TABLE IF EXISTS user_metadata;
        DROP TABLE IF EXISTS personal_data_processing;
        ";

        Connection.Execute(dropMetadataStatement);
    }

    public void MarkAsPersonalData(GDPRMetadata? metadata)
    {
        if (metadata.TargetTable is null || metadata.TargetColumn is null)
            throw new ArgumentException("TargetTable and TargetColumn must be defined");

        ColumnMetadata targetColumn = GetOrCreateColumn(metadata.TargetTable, metadata.TargetColumn);

        var insertedMetadata = _context.metadata.Add(new GdprMetadataEntity()
        {
            Column = targetColumn, Purpose = metadata.Purpose, Origin = metadata.Origin,
            LegallyRequired = metadata.LegallyRequired
        });

        _context.SaveChanges();
        string addReferencesToMetadataStatement = $@"
                        INSERT INTO user_metadata (user_id, metadata_id, objection, automated_decisionmaking) 
	                    SELECT id, {insertedMetadata.Entity.Id}, false, false from {_individualsTable}
                    ";
                    
        Connection.Execute(addReferencesToMetadataStatement);
    }

    private ColumnMetadata GetOrCreateColumn(string targetTable, string targetColumn)
    {
        // Get column from data if it exists
        ColumnMetadata? existingColumn = _context.columns.FirstOrDefault(col =>
            col.TargetColumn == targetTable && col.TargetColumn == targetColumn);
        if (existingColumn is not null) 
            return existingColumn;
        
        // Insert it if it does not exist
        ColumnMetadata column = new() { TargetColumn = targetColumn, TargetTable = targetTable };
        return _context.columns.Add(column).Entity;

    }

    /// <summary>
    ///     Updates the row in the table gdpr_metadata with the given entryId.
    ///     Only columns where the corresponding property is not null value is updated.
    /// </summary>
    /// <param name="entryId">The id of the row</param>
    /// <param name="value">Object defining the updated values</param>
    public void UpdateMetadataEntry(int entryId, GDPRMetadata value)
    {
        var existingMetadata = _context.metadata.FirstOrDefault(e => e.Id == entryId);
        
        // If there is no entry with the given id, do nothing
        if (existingMetadata is null)
            return;
        
        existingMetadata.Purpose = value.Purpose ?? existingMetadata.Purpose;
        existingMetadata.Origin = value.Origin ?? existingMetadata.Origin;
        existingMetadata.LegallyRequired = value.LegallyRequired ?? existingMetadata.LegallyRequired;

        _context.SaveChanges();
    }

    public void DeleteMetadataEntry(int entryId)
    {
        var entryToDelete = _context.metadata.FirstOrDefault(e => e.Id == entryId);
        // If there is no entry with the given id, do nothing
        if (entryToDelete is null)
            return;
        
        _context.Remove(entryToDelete);
        _context.SaveChanges();
    }

    public IEnumerable<GDPRMetadata> GetAllMetadataEntries()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Returns GDPRMetadata with matching id, or null if no such entry exists
    /// </summary>
    /// <param name="entryId">Id of the metadata</param>
    /// <returns>GDPRMetadata entry with given id</returns>
    public GDPRMetadata? GetMetadataEntry(int entryId)
    {
        GdprMetadataEntity? entry = _context.metadata
            .Include(entry => entry.Column)
            .FirstOrDefault(entry => entry.Id == entryId);

        return entry is null ? null : MapMetadata(entry);
    }

    
    private static GDPRMetadata MapMetadata(GdprMetadataEntity entry)
    {
        // Maps from the EFCore entity GdprMetadataEntity to the Domain Entity GDPRMetadata
        GDPRMetadata? result = new GDPRMetadata()
        {
            TargetTable = entry.Column.TargetTable, TargetColumn = entry.Column.TargetColumn, Purpose = entry.Purpose,
            Origin = entry.Origin, LegallyRequired = entry.LegallyRequired
        };
        return result;
    }

    public IEnumerable<GDPRMetadata> GetMetadataWithNullValues()
    {
        var res = _context.metadata.Where(e =>
            e.Purpose == null ||
            e.Origin == null ||
            e.LegallyRequired == null
        ).AsEnumerable().Select(MapMetadata).ToList();

        return res.Any() ? res : new List<GDPRMetadata>();
    }

    public void Dispose()
    {
        SqliteConnection.ClearPool((SqliteConnection) _context.Connection);
        _context.Dispose();
    }
}