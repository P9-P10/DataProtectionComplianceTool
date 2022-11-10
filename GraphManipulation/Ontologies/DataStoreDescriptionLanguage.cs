using VDS.RDF;

namespace GraphManipulation.Ontologies;

public static class DataStoreDescriptionLanguage
{
    public static Uri OntologyUri =>
        UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");
    
    // Predicates
    public static string HasStructure => "ddl:hasStructure";
    public static string HasDataType => "ddl:hasDataType";
    public static string HasStore => "ddl:hasStore";
    public static string StoredIn => "ddl:storedIn";
    public static string HasName => "ddl:hasName";
    public static string HasValue => "ddl:hasValue";
    public static string WithConnection => "ddl:withConnection";
    public static string PrimaryKey => "ddl:primaryKey";
    public static string ForeignKey => "ddl:foreignKey";
    public static string References => "ddl:references";
    public static string IsNotNull => "ddl:isNotNull";
    public static string ColumnOptions => "ddl:columnOptions";
    public static string ForeignKeyOnDelete => "ddl:foreignKeyOnDelete";
    public static string ForeignKeyOnUpdate => "ddl:foreignKeyOnUpdate";
    
    // Classes
    public static string Sqlite => "ddl:SQLite";
    public static string Schema => "ddl:Schema";
    public static string Table => "ddl:Table";
    public static string Column => "ddl:Column";
}