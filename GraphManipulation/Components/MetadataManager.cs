using System.Data;
using VDS.RDF;

namespace GraphManipulation.Components;

public class MetadataManager
{
    private IDbConnection _connection;

    public MetadataManager(IDbConnection connection)
    {
        _connection = connection;
    }

    public void MarkAsPersonalData(Uri uri)
    {
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