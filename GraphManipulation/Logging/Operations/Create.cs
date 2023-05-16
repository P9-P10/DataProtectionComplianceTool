using GraphManipulation.Helpers;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Logging.Operations;

public class Create<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Create(TKey key) : base(SystemAction.Operation.Created, key)
    {
    }
}