using GraphManipulation.Models.Base;

namespace GraphManipulation.Logging.Operations;

public class Update<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Update(TKey key, TValue value) : base(OperationName.Update, key, value)
    {
    }
}