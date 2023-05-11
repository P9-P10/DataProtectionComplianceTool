namespace GraphManipulation.Logging.Operations;

public class Create : Operation
{
    public Create(string type, string key, Dictionary<string ,string>? parameters) : base("Create", type, key)
    {
        Parameters = parameters;
    }
}