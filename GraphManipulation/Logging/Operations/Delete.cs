using GraphManipulation.Models.Base;

namespace GraphManipulation.Logging.Operations;

public class Delete<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Delete(TKey key) : base(OperationName.Delete, key)
    {
    }
}