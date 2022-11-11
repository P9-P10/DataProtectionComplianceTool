using VDS.RDF;

namespace GraphManipulation.Ontologies;

public static class DataStoreDescriptionLanguage
{
    public static string OntologyPrefix = "ddl";
    public static Uri OntologyUri =>
        UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");
    
    // Predicates
    public static string HasStructure => $"{OntologyPrefix}:hasStructure";
    public static string HasDataType => $"{OntologyPrefix}:hasDataType";
    public static string HasStore => $"{OntologyPrefix}:hasStore";
    public static string StoredIn => $"{OntologyPrefix}:storedIn";
    public static string HasName => $"{OntologyPrefix}:hasName";
    public static string HasValue => $"{OntologyPrefix}:hasValue";
    public static string WithConnection => $"{OntologyPrefix}:withConnection";
    public static string PrimaryKey => $"{OntologyPrefix}:primaryKey";
    public static string ForeignKey => $"{OntologyPrefix}:foreignKey";
    public static string References => $"{OntologyPrefix}:references";
    public static string IsNotNull => $"{OntologyPrefix}:isNotNull";
    public static string ColumnOptions => $"{OntologyPrefix}:columnOptions";
    public static string ForeignKeyOnDelete => $"{OntologyPrefix}:foreignKeyOnDelete";
    public static string ForeignKeyOnUpdate => $"{OntologyPrefix}:foreignKeyOnUpdate";
    
    // Classes
    public static string Sqlite => $"{OntologyPrefix}:SQLite";
    public static string Schema => $"{OntologyPrefix}:Schema";
    public static string Table => $"{OntologyPrefix}:Table";
    public static string Column => $"{OntologyPrefix}:Column";
}