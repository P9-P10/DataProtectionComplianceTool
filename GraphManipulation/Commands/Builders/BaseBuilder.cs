using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Commands.Builders;

public static class BaseBuilder
{
    public static ICommandHandler BuildHandlerWithKey<TValue, TKey>(
        IConsole console,
        IGetter<TValue, TKey> getter,
        Option<TKey> keyOption,
        string failureSubject,
        params Action<InvocationContext, TValue>[] actions)
        where TValue : IListable
    {
        return new CommandHandler()
            .WithHandle(context =>
            {
                var key = GetKey(context, keyOption);
                var value = getter.Get(key);

                if (value is null)
                {
                    console.WriteLine(CommandBuilder.BuildFailureToFindMessage(failureSubject, key));
                    return;
                }

                foreach (var action in actions)
                {
                    action(context, value);
                }
            });
    }

    public static TKey GetKey<TKey>(InvocationContext context, Option<TKey> keyOption)
    {
        if (!context.ParseResult.HasOption(keyOption))
        {
            throw new CommandException($"{keyOption.Name} missing from context");
        }

        return context.ParseResult.GetValueForOption(keyOption)!;
    }

    public static void CompareAndRun<TValue>(InvocationContext context, TValue compare, Option<TValue> option,
        Action<TValue> action)
        where TValue : notnull
    {
        if (!context.ParseResult.HasOption(option))
        {
            return;
        }

        var value = context.ParseResult.GetValueForOption(option)!;

        if (!compare.Equals(value))
        {
            action(value);
        }
    }
}