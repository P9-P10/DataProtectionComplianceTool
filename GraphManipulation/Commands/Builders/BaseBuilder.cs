using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;
using VDS.Common.Tries;
using VDS.RDF.Parsing.Tokens;

namespace GraphManipulation.Commands.Builders;

public static class BaseBuilder
{
    public static void AddHandler<TKey, T1, T2>(InvocationContext context,
        Action<TKey, T1, T2> addAction, Option<TKey> keyOption, Option<T1> option1, Option<T2> option2)
    {
        var key = GetKey(context, keyOption);
        
        if (!TryGetValueOfOption(context, option1, out var value1))
        {
            throw new CommandException($"{option1.Name} missing from context");
        }
        if (!TryGetValueOfOption(context, option2, out var value2))
        {
            throw new CommandException($"{option2.Name} missing from context");
        }

        addAction(key, value1, value2);
    }
    
    public static void UpdateHandler<TKey, TValue, T>(InvocationContext context, IConsole console,
        Action<TKey, T> updateAction, IGetter<TValue, TKey> getter, Func<TValue, T> getOld, Option<TKey> keyOption,
        Option<T> option)
        where TValue : IListable
    {
        var key = GetKey(context, keyOption);

        if (!TryGet(getter, key, out var keyValue))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key));
            return;
        }

        if (!TryGetValueOfOption(context, option, out var optionValue))
        {
            return;
        }

        if (!getOld(keyValue).Equals(optionValue))
        {
            updateAction(key, optionValue);
        }
    }

    public static void UpdateHandlerKeyRequired<TKey1, TValue1, TKey2, TValue2>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, TKey2> getOld,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2)
        where TValue1 : IListable where TValue2 : IListable
    {
        var key1 = GetKey(context, keyOption1);
        var key2 = GetKey(context, keyOption2);
        
        if (!TryGet(getter1, key1, out var keyValue1))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key1));
            return;
        }
        
        if (!TryGet(getter2, key2, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key2));
            return;
        }

        if (!getOld(keyValue1).Equals(key2))
        {
            updateAction(key1, key2);
        }
    }
    
    public static void UpdateHandlerKey<TKey1, TValue1, TKey2, TValue2>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, TKey2> getOld,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2)
        where TValue1 : IListable where TValue2 : IListable
    {
        var key1 = GetKey(context, keyOption1);

        if (!TryGet(getter1, key1, out var keyValue1))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key1));
            return;
        }

        if (!TryGetValueOfOption(context, keyOption2, out var key2))
        {
            return;
        }

        if (!TryGet(getter2, key2!, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key2));
            return;
        }

        if (!getOld(keyValue1).Equals(key2))
        {
            updateAction(key1, key2);
        }
    }

    public static void DeleteHandler<TValue, TKey>(InvocationContext context, IConsole console, Action<TKey> deleteAction,
        IGetter<TValue, TKey> getter, Option<TKey> keyOption)
        where TValue : IListable
    {
        var key = GetKey(context, keyOption);

        if (!TryGet(getter, key, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key));
            return;
        }

        deleteAction(key);
    }

    public static void ListHandler<TValue, _>(IConsole console, IGetter<TValue, _> getter)
        where TValue : IListable
    {
        getter.GetAll().ToList().ForEach(e => console.WriteLine(e.ToListing()));
    }

    public static void ShowHandler<TValue, TKey>(InvocationContext context, IConsole console, IGetter<TValue, TKey> getter, Option<TKey> keyOption)
        where TValue : IListable
    {
        var key = GetKey(context, keyOption);

        if (!TryGet(getter, key, out var keyValue))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key));
            return;
        }
        
        console.WriteLine(keyValue.ToListing());
    }
    

    public static TKey GetKey<TKey>(InvocationContext context, Option<TKey> keyOption)
    {
        if (TryGetValueOfOption(context, keyOption, out var key))
        {
            return key!;
        }

        throw new CommandException($"{keyOption.Name} missing from context");
    }

    public static bool TryGetValueOfOption<T>(InvocationContext context, Option<T> option, out T? value)
    {
        value = context.ParseResult.GetValueForOption(option);
        return context.ParseResult.HasOption(option);
    }

    public static bool TryGet<TValue, TKey>(IGetter<TValue, TKey> getter, TKey key, out TValue? value)
        where TValue : IListable
    {
        value = getter.Get(key);
        return value is not null;
    }
}