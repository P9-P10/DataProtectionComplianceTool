using GraphManipulation.Models.Base;

namespace GraphManipulation.Logging.Operations;

public class Execute<TKey, TValue> : Operation<TKey, TValue> where TValue : Entity<TKey>
{
    public Execute(TKey key) : base(OperationName.Execute, key)
    {
    }
}