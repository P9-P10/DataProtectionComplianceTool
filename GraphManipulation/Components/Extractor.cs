using System.Data.Common;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using VDS.RDF;

namespace GraphManipulation.Components;

public class Extractor<T> where T : Database
{
    private readonly T _database;

    public Extractor(DbConnection connection, string baseUri)
    {
        // TODO: Det her er åbenbart langsomt, men jeg kunne ikke finde andre måder der virkede
        _database = (T)Activator.CreateInstance(typeof(T), "", baseUri, connection)!;
    }

    public void Extract()
    {
        _database.BuildFromDataSource();
    }

    public IGraph GetDatabaseSchema()
    {
        return _database.ToGraph();
    }
}