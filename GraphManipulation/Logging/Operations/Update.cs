using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Logging.Operations;

public class Update<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Update(TKey key, TValue value) : base(SystemOperation.Operation.Updated, key, value)
    {
    }
}