using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Managers.Interfaces;
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

    protected IEnumerable<TV> HandleMustExistList<TK, TV>(IEnumerable<TK> keys, IManager<TK, TV> manager)
    {
        return keys.Select(key => HandleMustExist(key, manager));
    }

    protected TV HandleMustExist<TK, TV>(TK key, IManager<TK, TV> manager)
    {
        var value = manager.Get(key);
        if (value is not null)
        {
            return value;
        }
        
        throw new BindingException($"Could not bind to {key} as it does not exist in the system");
    }
}

public class BindingException : Exception
{
    public BindingException(string message) : base(message)
    {
        
    }
}