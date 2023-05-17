using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Logging.Operations;

public class Execute<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Execute(TKey key) : base(SystemOperation.Operation.Executed, key)
    {
    }
}