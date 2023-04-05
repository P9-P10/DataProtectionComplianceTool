using System.Data;
using System.Reflection;
using Dapper;
using Dapper.Transaction;
using GraphManipulation.MetadataManagement.AttributeMapping;

namespace GraphManipulation.MetadataManagement;

public class PropWithAttribute<T> where T : Attribute
{
    public PropWithAttribute(PropertyInfo prop, T attr)
    {
        Property = prop;
        Attribute = attr;
    }

    public PropertyInfo Property { get; }
    public T Attribute { get; }
}

public class MetadataManager : IMetadataManager
{
    private readonly IDbConnection _connection;
    private readonly string _individualsTable;

    /// <summary>
    ///     Provides methods for interacting with GDPR metadata stored in a database represented by the given IDbConnection.
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
        
        create table metadata_purposes(
               id integer primary key autoincrement,
               purpose varchar
               );

        create table metadata_origins(
               id integer primary key autoincrement,
               origin varchar
               );
        
        CREATE TABLE gdpr_metadata(
               id INTEGER PRIMARY KEY AUTOINCREMENT,
               purpose INTEGER,
               target_column INTEGER,
               origin INTEGER,
               legally_required INTEGER,
               FOREIGN KEY (purpose) REFERENCES metadata_purposes(id),
               FOREIGN KEY (target_column) REFERENCES metadata_columns(id),
               FOREIGN KEY (origin) REFERENCES metadata_origins(id)
               );
        
        CREATE TABLE delete_conditions (
               id INTEGER PRIMARY KEY AUTOINCREMENT,
               metadata_id INTEGER,
               condition VARCHAR,
               FOREIGN KEY (metadata_id) REFERENCES gdpr_metadata(id)
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
        var dropMetadataStatement = @"
        DROP TABLE IF EXISTS metadata_columns;
        drop table if exists metadata_purposes;
        drop table if exists metadata_origins;
        DROP TABLE IF EXISTS gdpr_metadata;
        DROP TABLE IF EXISTS delete_conditions;
        DROP TABLE IF EXISTS user_metadata;
        DROP TABLE IF EXISTS personal_data_processing;
        ";

        _connection.Execute(dropMetadataStatement);
    }

    public void MarkAsPersonalData(GDPRMetadata metadata)
    {
        _connection.Open();
        using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    Dictionary<string, int> definedColumns = new Dictionary<string, int>();
                    // Target Column
                    if (metadata.TargetTable == null || metadata.TargetColumn == null)
                    {
                        throw new ArgumentException(
                            "Instance of GDPRMetadata must define properties TargetTable and TargetColumn");
                    }
                    else
                    {
                        int columnId = getRowId(transaction, new []{metadata.TargetTable, metadata.TargetColumn}, new []{"target_table", "target_column"}, "metadata_columns");
                        definedColumns.Add("target_column", columnId);
                    }
                    // Purpose
                    if (metadata.Purpose != null)
                    {
                        int purposeId = getRowId(transaction, metadata.Purpose, "purpose", "metadata_purposes");
                        definedColumns.Add("purpose", purposeId);
                    }
                    // Origin
                    if (metadata.Origin != null)
                    {
                        int originId = getRowId(transaction, metadata.Origin, "origin", "metadata_origins");
                        definedColumns.Add("origin", originId);
                    }
                    // Legally required
                    if (metadata.LegallyRequired != null)
                    {
                        // Convert bool to integer
                        definedColumns.Add("legally_required", metadata.LegallyRequired == true ? 1 : 0);
                    }

                    string columns = string.Join(", ", definedColumns.Keys);
                    string values = string.Join(", ", definedColumns.Values.Select(val => val.ToString()));
                    
                    string insertMetadataQuery = $@"
                         INSERT INTO gdpr_metadata ({columns})
                         VALUES ({values});
                        SELECT last_insert_rowid();
                    ";
                    int metadataId = transaction.QuerySingle<int>(insertMetadataQuery);
                    
                    string addReferencesToMetadataStatement = $@"
                        INSERT INTO user_metadata (user_id, metadata_id, objection, automated_decisionmaking) 
	                    SELECT id, {metadataId}, false, false from {_individualsTable}
                    ";
                    
                    transaction.Execute(addReferencesToMetadataStatement);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    // Rollback the transaction in case of an exception
                    // A more specific exception would be preferred, but the exception seems to be specific to sqlite
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }

            }
    }

    private int getRowId(IDbTransaction transaction, object? value, string column, string table)
    {
        return getRowId(transaction, new[] { value }, new[] { column }, table);
    }
    
    private int getRowId(IDbTransaction transaction, IEnumerable<object?> values, IEnumerable<string> columns, string table)
    {
        int? id = null;
        if (values.All(val => val != null))
        {
            // Get id of existing purpose
            string conditionString =
                string.Join(" AND ", columns.Zip(values, (column, value) => $"{column} = '{value}'"));
            id = transaction
                .Query<int?>($"SELECT id FROM {table} WHERE { conditionString }").FirstOrDefault();
            // If purpose does not already exist, create it
            if (id == null)
            {
                string valuesString = string.Join(", ", values.Select(val => $"'{val.ToString()}'"));
                id = transaction
                    .QuerySingle<int>(
                        $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES ({valuesString}); SELECT last_insert_rowid();");
            }
        }

        return id.Value;
    }

    /// <summary>
    ///     Updates the row in the table gdpr_metadata with the given entryId.
    ///     Only columns where the corresponding property is not null value is updated.
    /// </summary>
    /// <param name="entryId">The id of the row</param>
    /// <param name="value">Object defining the updated values</param>
    public void UpdateMetadataEntry(int entryId, GDPRMetadata value)
    {
        _connection.Open();
        using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    Dictionary<string, int> definedColumns = new Dictionary<string, int>();
                    if (value.Purpose != null)
                    {
                        int purposeId = getRowId(transaction, value.Purpose, "purpose", "metadata_purposes");
                        definedColumns.Add("purpose", purposeId);
                    }

                    if (value.Origin != null)
                    {
                        int originId = getRowId(transaction, value.Origin, "origin", "metadata_origins");
                        definedColumns.Add("origin", originId);
                    }

                    if (value.LegallyRequired != null)
                    {
                        // Need to convert boolean to integer
                        definedColumns.Add("legally_required", value.LegallyRequired == true ? 1 : 0);
                    }

                    IEnumerable<string> setValues = definedColumns.Select(kv => $"{kv.Key} = {kv.Value.ToString()}");
                    string valuesString = string.Join(", ", setValues);

                    string updateStatement = $@"
                        UPDATE gdpr_metadata 
                        SET {valuesString}
                        WHERE id = {entryId}
                        ";

                    transaction.Execute(updateStatement);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    // Rollback the transaction in case of an exception
                    // A more specific exception would be preferred, but the exception seems to be specific to sqlite
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }

            }
    }

    private static IEnumerable<string> CreateValuesUpdates(GDPRMetadata value)
    {
        // Get PropWithAttribute for properties that are not null
        var propsWithAttributes =
            GetPropertiesWithAttributes<GDPRMetadata, ColumnAttribute>()
                .Where(pair => pair.Property.GetValue(value) != null);

        var columns = GetColumnNames(propsWithAttributes);
        var values = GetColumnValues(value, propsWithAttributes);

        var valueUpdates = columns.Zip(values, (col, val) => $"{col} = {val}");
        return valueUpdates;
    }

    private static IEnumerable<string> GetColumnNames(IEnumerable<PropWithAttribute<ColumnAttribute>> result)
    {
        var columns = result.Select(pair => pair.Attribute.Name);
        return columns;
    }

    private static IEnumerable<string> GetColumnValues(GDPRMetadata value,
        IEnumerable<PropWithAttribute<ColumnAttribute>> result)
    {
        // Get the value of the properties as strings.
        // If the value is already a string, it must be surrounded by singlequotes
        var values = result
            .Select(pair => pair.Property.GetValue(value))
            .Select(val => val is string ? $"'{val}'" : $"{val.ToString().ToLower()}");
        return values;
    }

    public GDPRMetadata GetMetadataEntry(int entryId)
    {
        var result = _connection.QuerySingle<GDPRMetadata>(
            $@"SELECT cols.target_table, cols.target_column, purposes.purpose, origins.origin, md.legally_required 
                  FROM gdpr_metadata AS md 
                  LEFT JOIN metadata_columns AS cols ON md.target_column = cols.id
                  LEFT JOIN metadata_purposes AS purposes on md.purpose = purposes.id
                  LEFT JOIN metadata_origins AS origins on md.origin = origins.id
                  WHERE md.id = {entryId}");

        return result;
    }

    public IEnumerable<GDPRMetadata> GetMetadataWithNullValues()
    {
        IEnumerable<string> columns = new[] { "id", "purpose", "target_column", "origin" };
        var result = _connection.Query<int>(@$"
            select id from gdpr_metadata 
            where {string.Join(" IS NULL OR ", columns)} IS NULL");

        if (result.Any())
            return result.Select(GetMetadataEntry).ToList();
        else
            return new List<GDPRMetadata>();
    }

    /// <summary>
    ///     Returns an IEnumerable<PropWithAttribute> for the properties of T1 that have the attribute T2
    /// </summary>
    /// <typeparam name="T1">Type with properties with attributes</typeparam>
    /// <typeparam name="T2">Type of the attributes</typeparam>
    /// <returns></returns>
    private static IEnumerable<PropWithAttribute<T2>> GetPropertiesWithAttributes<T1, T2>() where T2 : Attribute
    {
        return typeof(T1).GetProperties()
            .Where(prop => prop.GetCustomAttribute<T2>(true) != null)
            .Select(prop => new PropWithAttribute<T2>(prop, prop.GetCustomAttribute<T2>()));
    }
}