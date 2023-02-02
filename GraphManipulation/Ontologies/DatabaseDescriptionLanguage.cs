using VDS.RDF;

namespace GraphManipulation.Ontologies;

public static class DatabaseDescriptionLanguage
{
    public const string OntologyPrefix = "ddl";

    // Predicates
    public const string HasStructure = $"{OntologyPrefix}:hasStructure";
    public const string HasDataType = $"{OntologyPrefix}:hasDataType";
    public const string HasStore = $"{OntologyPrefix}:hasStore";
    public const string StoredIn = $"{OntologyPrefix}:storedIn";
    public const string HasName = $"{OntologyPrefix}:hasName";
    public const string HasValue = $"{OntologyPrefix}:hasValue";
    public const string WithConnection = $"{OntologyPrefix}:withConnection";
    public const string PrimaryKey = $"{OntologyPrefix}:primaryKey";
    public const string ForeignKey = $"{OntologyPrefix}:foreignKey";
    public const string References = $"{OntologyPrefix}:references";
    public const string IsNotNull = $"{OntologyPrefix}:isNotNull";
    public const string ColumnOptions = $"{OntologyPrefix}:columnOptions";
    public const string ForeignKeyOnDelete = $"{OntologyPrefix}:foreignKeyOnDelete";
    public const string ForeignKeyOnUpdate = $"{OntologyPrefix}:foreignKeyOnUpdate";

    // Classes
    public const string Datastore = $"{OntologyPrefix}:Datastore";
    public const string Sqlite = $"{OntologyPrefix}:SQLite";
    public const string Schema = $"{OntologyPrefix}:Schema";
    public const string Table = $"{OntologyPrefix}:Table";
    public const string Column = $"{OntologyPrefix}:Column";

    public static Uri OntologyUri =>
        UriFactory.Create("http://www.cs-22-dt-9-03.org/database-description-language#");
}