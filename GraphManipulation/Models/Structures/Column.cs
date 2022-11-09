using VDS.RDF;
using VDS.RDF.Parsing;

namespace GraphManipulation.Models.Structures;

public class Column : Structure
{
    public static readonly List<string> ValidOptions = new() { "AUTOINCREMENT" };

    public Column(string name, string dataType = "", bool isNotNull = false, string options = "") : base(name)
    {
        DataType = dataType;
        IsNotNull = isNotNull;
        Options = options;
    }

    public string Options { get; private set; }
    public string DataType { get; private set; }
    public bool IsNotNull { get; private set; }

    public void SetDataType(string dataType)
    {
        DataType = dataType;
    }

    public void SetIsNotNull(bool isNotNull)
    {
        IsNotNull = isNotNull;
    }

    public void SetOptions(string options)
    {
        Options = options;
    }
}