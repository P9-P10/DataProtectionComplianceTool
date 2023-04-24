namespace GraphManipulation.Logging.Operations;

public class Add : Operation
{
    public Add(string type, string key, Dictionary<string ,string>? parameters) : base("Add", type, key)
    {
        Parameters = parameters;
    }
}