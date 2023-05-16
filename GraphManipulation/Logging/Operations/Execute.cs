using GraphManipulation.Helpers;
using GraphManipulation.Models;

namespace GraphManipulation.Logging.Operations;

public class Execute<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Execute(TKey key) : base(SystemAction.Operation.Executed, key)
    {
    }
}