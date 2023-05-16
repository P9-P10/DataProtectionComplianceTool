using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Models;

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
        string? description = null;

        if (bindingContext.ParseResult.HasOption(_descriptionOption))
        {
            description = bindingContext.ParseResult.GetValueForOption(_descriptionOption);
        }
        
        return new TValue { Key = key, Description = description };
    }

    protected IEnumerable<TV> HandleMustExistList<TK, TV>(IEnumerable<TK> keys, IManager<TK, TV> manager)
    {
        return keys.Select(key => HandleMustExist(key, manager)).ToList();
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

    protected IEnumerable<TV> HandleMustExistListWithCreateOnDemand<TK, TV>(IEnumerable<TK> keys,
        IManager<TK, TV> manager) where TV : Entity<TK>
    {
        return keys.Select(key => HandleMustExistWithCreateOnDemand(key, manager)).ToList();
    }

    protected TV HandleMustExistWithCreateOnDemand<TK, TV>(TK key, IManager<TK, TV> manager)
        where TV : Entity<TK>
    {
        try
        {
            return HandleMustExist(key, manager);
        }
        catch (BindingException)
        {
            if (!PromptCreateNew<TK, TV>(key))
            {
                throw;
            }

            Handler<TK, TV>.CreateHandler(key, manager, new FeedbackEmitter<TK, TV>(),
                _ => Console.WriteLine("Not reporting status when creating on demand"));
            var value = manager.Get(key);
            return value!;
        }
    }

    private static bool PromptCreateNew<TK, TV>(TK key)
        where TV : Entity<TK>
    {
        while (true)
        {
            Console.Write(
                $"{key} {TypeToString.GetEntityType(typeof(TV))} does not exist. Would you like to create one? (y/n){Environment.NewLine}$: ");
            var reply = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrEmpty(reply))
            {
                Console.WriteLine("You must answer either 'y' or 'n'");
            }
            else
                switch (reply)
                {
                    case "y" or "Y":
                        return true;
                    case "n" or "N":
                        return false;
                    default:
                        Console.WriteLine($"Cannot parse '{reply}', you must either answer 'y' or 'n'");
                        break;
                }
        }
    }
}

public class BindingException : Exception
{
    public BindingException(string message) : base(message)
    {
    }
}