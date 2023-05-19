using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Binders;

public abstract class BaseBinder<TKey, TValue> : BinderBase<TValue> where TValue : Entity<TKey>, new()
{
    private readonly Option<string> _descriptionOption;
    private readonly Option<TKey> _keyOption;

    protected BaseBinder(Option<TKey> keyOption, Option<string> descriptionOption)
    {
        _keyOption = keyOption;
        _descriptionOption = descriptionOption;
    }

    protected override TValue GetBoundValue(BindingContext bindingContext)
    {
        var key = bindingContext.ParseResult.GetValueForOption(_keyOption)!;
        string? description = null;

        if (bindingContext.ParseResult.HasOption(_descriptionOption))
        {
            description = bindingContext.ParseResult.GetValueForOption(_descriptionOption);
        }

        return new TValue { Key = key, Description = description };
    }
}