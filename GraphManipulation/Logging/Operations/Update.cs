namespace GraphManipulation.Logging.Operations;

public class Update : Operation
{
    public Update(string type, string key, Dictionary<string ,string>? parameters) : base("Update", type, key)
    {
        Parameters = parameters;
    }
}