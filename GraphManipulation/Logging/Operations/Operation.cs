namespace GraphManipulation.Logging.Operations;

public abstract class Operation
{
    public string Name { get; } // The name of the operation
    public string Type { get; } // The type being operated on
    public string Key { get; } // The identifier of the subject
    public Dictionary<string, string>? Parameters { get; protected set; } // The parameters of the operation

    public Operation(string name, string type, string key)
    {
        Name = name;
        Type = type;
        Key = key;
    }
    
    public override string ToString()
    {
        string parameters =  Parameters is null ? "" : " Parameters: " + string.Join(", ", Parameters);
        return $"{Name} {Type} {Key}." + parameters;
    }
}