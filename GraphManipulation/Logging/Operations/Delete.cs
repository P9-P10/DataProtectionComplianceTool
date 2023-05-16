using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Logging.Operations;

public class Delete<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Delete(TKey key) : base(SystemOperation.Operation.Deleted, key)
    {
    }
}