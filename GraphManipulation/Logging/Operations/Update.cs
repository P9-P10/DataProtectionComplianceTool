using GraphManipulation.Helpers;
using GraphManipulation.Models;

namespace GraphManipulation.Logging.Operations;

public class Update<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Update(TKey key, TValue value) : base(SystemAction.Operation.Updated, key, value)
    {
    }
}