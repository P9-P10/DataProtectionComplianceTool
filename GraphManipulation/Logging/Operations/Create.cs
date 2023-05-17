using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Logging.Operations;

public class Create<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Create(TKey key) : base(SystemOperation.Operation.Created, key)
    {
    }
}