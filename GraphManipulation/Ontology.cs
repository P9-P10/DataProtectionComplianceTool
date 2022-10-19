using VDS.RDF;

namespace GraphManipulation;

public class Ontology
{
    private readonly string _path;
    private readonly IRdfReader _ontologyReader;

    public Ontology(string path, IRdfReader reader)
    {
        _path = path;
        _ontologyReader = reader;
    }

    public void ToGraph(IGraph result)
    {
        _ontologyReader.Load(result, _path);
    }
}