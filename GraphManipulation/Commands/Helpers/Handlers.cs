using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Commands.Helpers;

public static class Handlers
{
    public static void AddHandler<TKey, T1, T2>(InvocationContext context, IConsole console,
        Action<TKey, T1, T2> addAction, Option<TKey> keyOption, Option<T1> option1, Option<T2> option2)
    {
        var key = GetValueOfRequiredOption(context, keyOption);
        var value1 = GetValueOfRequiredOption(context, option1);
        var value2 = GetValueOfRequiredOption(context, option2);

        addAction(key, value1, value2);

        console.WriteLine($"{key} successfully added");
    }

    public static void AddHandler<T>(InvocationContext context, IConsole console,
        Action<T> addAction, Option<T> option)
    {
        var key = GetValueOfRequiredOption(context, option);
        addAction(key);

        console.WriteLine($"{key} successfully added");
    }

    public static void SetHandlerKey<TKey1, TValue1, TKey2, TValue2, TKey3, TValue3>(InvocationContext context,
        IConsole console,
        Action<TKey1, TKey2, TKey3> setAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        IGetter<TValue3, TKey3> getter3,
        Func<TKey1, TKey2, TKey3> getCurrent,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2,
        Option<TKey3> keyOption3)
        where TValue1 : IListable where TValue2 : IListable where TValue3 : IListable
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        var key2 = GetValueOfRequiredOption(context, keyOption2);
        var key3 = GetValueOfRequiredOption(context, keyOption3);

        if (!TryGet(getter1, key1, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key1));
            return;
        }

        if (!TryGet(getter2, key2, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key2));
            return;
        }

        if (!TryGet(getter3, key3, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key3));
            return;
        }

        if (getCurrent(key1, key2).Equals(key3))
        {
            return;
        }

        setAction(key1, key2, key3);
        console.WriteLine($"Successfully completed set operation using {key1}, {key2}, {key3}");
    }

    public static void UpdateHandler<TKey, TValue, T>(InvocationContext context, IConsole console,
        Action<TKey, T> updateAction, IGetter<TValue, TKey> getter, Func<TValue, T> getOld, Option<TKey> keyOption,
        Option<T> option)
        where TValue : IListable
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (!TryGet(getter, key, out var keyValue))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key));
            return;
        }

        if (!TryGetValueOfOption(context, option, out var optionValue))
        {
            return;
        }

        if (getOld(keyValue).Equals(optionValue))
        {
            return;
        }

        updateAction(key, optionValue);
        console.WriteLine($"{key} successfully updated with {optionValue}");
    }

    public static void UpdateHandlerKeyRequired<TKey1, TValue1, TKey2, TValue2>(InvocationContext context,
        IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, TKey2> getOld,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2)
        where TValue1 : IListable where TValue2 : IListable
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        var key2 = GetValueOfRequiredOption(context, keyOption2);

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

        if (getOld(keyValue1).Equals(key2))
        {
            return;
        }

        updateAction(key1, key2);
        console.WriteLine($"{key1} successfully updated with {key2}");
    }

    public static void UpdateHandlerWithKey<TKey1, TValue1, TKey2, TValue2>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, TKey2> getOld,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2)
        where TValue1 : IListable where TValue2 : IListable
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);

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

        if (getOld(keyValue1).Equals(key2))
        {
            return;
        }

        updateAction(key1, key2);
        console.WriteLine($"{key1} successfully updated with {key2}");
    }

    public static void UpdateHandlerWithKeyList<TKey1, TValue1, TKey2, TValue2>(InvocationContext context,
        IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, IEnumerable<TKey2>> getOld,
        Option<TKey1> keyOption1,
        Option<IEnumerable<TKey2>> keyOption2)
        where TValue1 : IListable where TValue2 : IListable
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);

        if (!TryGet(getter1, key1, out var keyValue1))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key1));
            return;
        }

        if (!TryGetValueOfOption(context, keyOption2, out var key2List))
        {
            return;
        }

        foreach (var key2 in key2List)
            if (TryGet(getter2, key2, out _))
            {
                if (getOld(keyValue1).Contains(key2))
                {
                    continue;
                }

                updateAction(key1, key2);
                console.WriteLine($"{key1} successfully updated with {key2}");
            }
            else
            {
                console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key2));
            }
    }

    public static void RemoveHandlerKeyList<TKey1, TValue1, TKey2>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2> removeAction,
        IGetter<TValue1, TKey1> getter,
        Func<TValue1, IEnumerable<TKey2>> getCurrent,
        Option<TKey1> keyOption1,
        Option<IEnumerable<TKey2>> keyOption2)
        where TValue1 : IListable
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);

        if (!TryGet(getter, key1, out var keyValue1))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key1));
            return;
        }

        if (!TryGetValueOfOption(context, keyOption2, out var key2List))
        {
            return;
        }

        foreach (var key2 in key2List)
        {
            if (!getCurrent(keyValue1).Contains(key2))
            {
                console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key2));
                continue;
            }

            removeAction(key1, key2);
            console.WriteLine($"{key2} successfully removed from {key1}");
        }
    }

    public static void DeleteHandler<TValue, TKey>(InvocationContext context, IConsole console,
        Action<TKey> deleteAction,
        IGetter<TValue, TKey> getter, Option<TKey> keyOption)
        where TValue : IListable
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (!TryGet(getter, key, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key));
            return;
        }

        deleteAction(key);
        console.WriteLine($"{key} successfully deleted");
    }


    public static void ListHandler<TValue, _>(IConsole console, IGetter<TValue, _> getter)
        where TValue : IListable
    {
        getter.GetAll().ToList().ForEach(e => console.WriteLine(e.ToListing()));
    }

    public static void ShowHandler<TValue, TKey>(InvocationContext context, IConsole console,
        IGetter<TValue, TKey> getter,
        Option<TKey> keyOption)
        where TValue : IListable
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (!TryGet(getter, key, out var keyValue))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key));
            return;
        }

        console.WriteLine(keyValue.ToListing());
    }

    public static void ShowHandler<TValue1, TKey1, TValue2, TKey2, TValue3>(InvocationContext context, IConsole console,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TKey1, TKey2, TValue3> getCurrent,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2)
        where TValue1 : IListable where TValue2 : IListable where TValue3 : IListable
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        var key2 = GetValueOfRequiredOption(context, keyOption2);

        if (!TryGet(getter1, key1, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key1));
            return;
        }

        if (!TryGet(getter2, key2, out _))
        {
            console.WriteLine(CommandBuilder.BuildFailureToFindMessage("entity", key2));
            return;
        }

        console.WriteLine(getCurrent(key1, key2).ToListing());
    }


    private static TKey GetValueOfRequiredOption<TKey>(InvocationContext context, Option<TKey> keyOption)
    {
        if (TryGetValueOfOption(context, keyOption, out var key))
        {
            return key!;
        }

        throw new CommandException($"{keyOption.Name} missing from context");
    }

    private static bool TryGetValueOfOption<T>(InvocationContext context, Option<T> option, out T? value)
    {
        value = context.ParseResult.GetValueForOption(option);
        return context.ParseResult.HasOption(option);
    }

    private static bool TryGet<TValue, TKey>(IGetter<TValue, TKey> getter, TKey key, out TValue? value)
        where TValue : IListable
    {
        value = getter.Get(key);
        return value is not null;
    }
}