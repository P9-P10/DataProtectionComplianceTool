using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Builders.Binders;

public abstract class BaseBinder<TKey, TValue> : BinderBase<TValue> where TValue : Entity<TKey>, new()
{
    private readonly Option<TKey> _keyOption;
    private readonly Option<string> _descriptionOption;

    protected BaseBinder(Option<TKey> keyOption, Option<string> descriptionOption)
    {
        _keyOption = keyOption;
        _descriptionOption = descriptionOption;
    }

    protected override TValue GetBoundValue(BindingContext bindingContext)
    {
        var key = bindingContext.ParseResult.GetValueForOption(_keyOption)!;
        var description = bindingContext.ParseResult.GetValueForOption(_descriptionOption)!;

        return new TValue { Key = key, Description = description};
    }
}