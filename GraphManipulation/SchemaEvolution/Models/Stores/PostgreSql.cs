using System.Data.Common;

namespace GraphManipulation.SchemaEvolution.Models.Stores;

public class PostgreSql : Relational
{
    public PostgreSql(string name) : base(name)
    {
    }

    public PostgreSql(string name, string baseUri) : base(name, baseUri)
    {
    }

    public PostgreSql(string name, string baseUri, DbConnection connection) : base(name, baseUri, connection)
    {
    }

    protected override void GetStructureQueryResults()
    {
        throw new NotImplementedException();
    }

    protected override void GetForeignKeysQueryResults()
    {
        throw new NotImplementedException();
    }

    protected override void GetColumnOptionsQueryResults()
    {
        throw new NotImplementedException();
    }
}