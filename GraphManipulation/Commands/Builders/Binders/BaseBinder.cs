using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Builders.Binders;

public abstract class BaseBinder<TKey, TValue> : BinderBase<TValue> where TValue : Entity<TKey>, new()
{
    private readonly Option<TKey> _keyOption;

    protected BaseBinder(Option<TKey> keyOption)
    {
        _keyOption = keyOption;
    }

    protected override TValue GetBoundValue(BindingContext bindingContext)
    {
        var key = bindingContext.ParseResult.GetValueForOption(_keyOption)!;

        return new TValue { Key = key };
    }
}