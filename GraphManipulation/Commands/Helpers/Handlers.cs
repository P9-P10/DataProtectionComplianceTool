using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Commands.Helpers;

public static class Handlers
{
    public static void AddHandlerKey<TKey1, TValue1, TKey2, TValue2, T3>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2, T3> addAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2,
        Option<T3> option)
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        var key2 = GetValueOfRequiredOption(context, keyOption2);

        if (TryGet(getter1, key1, out _))
        {
            console.Error.WriteLine(AlreadyExistsMessage(key1, typeof(TValue1)));
            return;
        }

        if (!TryGet(getter2, key2, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key2, typeof(TValue2)));
            return;
        }

        if (!TryGetValueOfOption(context, option, out var optionValue))
        {
            return;
        }

        addAction(key1, key2, optionValue);
        console.WriteLine(SuccessMessage(key1, typeof(TValue1), Operations.Added, key2.ToString(),
            optionValue.ToString()));
    }

    public static void AddHandlerKey<TKey1, TValue1, TKey2, TValue2, TKey3, TValue3, T4>(InvocationContext context,
        IConsole console,
        Action<TKey1, TKey2, TKey3, T4> addAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        IGetter<TValue3, TKey3> getter3,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2,
        Option<TKey3> keyOption3,
        Option<T4> option)
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        var key2 = GetValueOfRequiredOption(context, keyOption2);
        var key3 = GetValueOfRequiredOption(context, keyOption3);

        if (TryGet(getter1, key1, out _))
        {
            console.Error.WriteLine(AlreadyExistsMessage(key1, typeof(TValue1)));
            return;
        }

        if (!TryGet(getter2, key2, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key2, typeof(TValue2)));
            return;
        }

        if (!TryGet(getter3, key3, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key3, typeof(TValue3)));
            return;
        }

        if (!TryGetValueOfOption(context, option, out var optionValue))
        {
            return;
        }

        addAction(key1, key2, key3, optionValue);
        console.WriteLine(SuccessMessage(key1, typeof(TValue1), Operations.Added, key2.ToString(), key3.ToString(),
            optionValue.ToString()));
    }

    public static void AddHandler<TKey, TValue>(InvocationContext context, IConsole console,
        Action<TKey> addAction,
        IGetter<TValue, TKey> getter,
        Option<TKey> keyOption)
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (TryGet(getter, key, out _))
        {
            console.Error.WriteLine(AlreadyExistsMessage(key, typeof(TValue)));
            return;
        }

        addAction(key);

        console.WriteLine(SuccessMessage(key, typeof(TValue), Operations.Added));
    }

    public static void AddHandler<TKey, TValue, T1>(InvocationContext context, IConsole console,
        Action<TKey, T1> addAction,
        IGetter<TValue, TKey> getter,
        Option<TKey> keyOption,
        Option<T1> option1)
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (TryGet(getter, key, out _))
        {
            console.Error.WriteLine(AlreadyExistsMessage(key, typeof(TValue)));
            return;
        }

        var value1 = GetValueOfRequiredOption(context, option1);
        addAction(key, value1);

        console.WriteLine(SuccessMessage(key, typeof(TValue), Operations.Added, value1.ToString()));
    }

    public static void AddHandler<TKey, TValue, T1, T2>(InvocationContext context, IConsole console,
        Action<TKey, T1, T2> addAction,
        IGetter<TValue, TKey> getter,
        Option<TKey> keyOption,
        Option<T1> option1,
        Option<T2> option2)
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (TryGet(getter, key, out _))
        {
            console.Error.WriteLine(AlreadyExistsMessage(key, typeof(TValue)));
            return;
        }

        var value1 = GetValueOfRequiredOption(context, option1);
        var value2 = GetValueOfRequiredOption(context, option2);

        addAction(key, value1, value2);

        console.WriteLine(SuccessMessage(key, typeof(TValue), Operations.Added, value1.ToString(), value2.ToString()));
    }

    public static void SetHandler<TKey>(InvocationContext context, IConsole console,
        Action<TKey> setAction,
        Option<TKey> keyOption)
    {
        var key = GetValueOfRequiredOption(context, keyOption);
        setAction(key);
        console.WriteLine($"{key} successfully set");
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
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        var key2 = GetValueOfRequiredOption(context, keyOption2);
        var key3 = GetValueOfRequiredOption(context, keyOption3);

        if (!TryGet(getter1, key1, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key1, typeof(TValue1)));
            return;
        }

        if (!TryGet(getter2, key2, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key2, typeof(TValue2)));
            return;
        }

        if (!TryGet(getter3, key3, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key3, typeof(TValue3)));
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
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (!TryGet(getter, key, out var keyValue))
        {
            console.Error.WriteLine(FailureToFindMessage(key, typeof(TValue)));
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
        console.WriteLine(SuccessMessage(key, typeof(TValue), Operations.Updated, optionValue.ToString()));
    }

    public static void UpdateHandlerUnique<TKey, TValue>(InvocationContext context, IConsole console,
        Action<TKey, TKey> updateAction, IGetter<TValue, TKey> getter, Func<TValue, TKey> getOld, Option<TKey> keyOption1,
        Option<TKey> keyOption2)
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);
        
        if (!TryGet(getter, key1, out var keyValue1))
        {
            console.Error.WriteLine(FailureToFindMessage(key1, typeof(TValue)));
            return;
        }
        
        if (!TryGetValueOfOption(context, keyOption2, out var key2))
        {
            return;
        }

        if (TryGet(getter, key2, out var keyValue2))
        {
            console.Error.WriteLine(AlreadyExistsMessage(key2, typeof(TValue)));
            return;
        }
        
        if (getOld(keyValue1).Equals(keyValue2))
        {
            return;
        }
        
        updateAction(key1, key2);
        console.WriteLine(SuccessMessage(key1, typeof(TValue), Operations.Updated, key2.ToString()));
    }

    public static void UpdateHandlerWithKey<TKey1, TValue1, TKey2, TValue2>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, TKey2> getOld,
        Option<TKey1> keyOption1,
        Option<TKey2> keyOption2)
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);

        if (!TryGet(getter1, key1, out var keyValue1))
        {
            console.Error.WriteLine(FailureToFindMessage(key1, typeof(TValue1)));
            return;
        }

        if (!TryGetValueOfOption(context, keyOption2, out var key2))
        {
            return;
        }

        if (!TryGet(getter2, key2!, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key2, typeof(TValue2)));
            return;
        }

        if (getOld(keyValue1).Equals(key2))
        {
            return;
        }

        updateAction(key1, key2);
        console.WriteLine(SuccessMessage(key1, typeof(TValue1), Operations.Updated, key2.ToString()));
    }

    public static void UpdateHandlerWithKeyList<TKey1, TValue1, TKey2, TValue2>(InvocationContext context,
        IConsole console,
        Action<TKey1, TKey2> updateAction,
        IGetter<TValue1, TKey1> getter1,
        IGetter<TValue2, TKey2> getter2,
        Func<TValue1, IEnumerable<TKey2>> getOld,
        Option<TKey1> keyOption1,
        Option<IEnumerable<TKey2>> keyOption2)
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);

        if (!TryGet(getter1, key1, out var keyValue1))
        {
            console.Error.WriteLine(FailureToFindMessage(key1, typeof(TValue1)));
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
                console.WriteLine(SuccessMessage(key1, typeof(TValue1), Operations.Updated, key2.ToString()));
            }
            else
            {
                console.Error.WriteLine(FailureToFindMessage(key2, typeof(TValue2)));
            }
    }

    public static void RemoveHandlerKeyList<TKey1, TValue1, TKey2>(InvocationContext context, IConsole console,
        Action<TKey1, TKey2> removeAction,
        IGetter<TValue1, TKey1> getter,
        Func<TValue1, IEnumerable<TKey2>> getCurrent,
        Option<TKey1> keyOption1,
        Option<IEnumerable<TKey2>> keyOption2)
    {
        var key1 = GetValueOfRequiredOption(context, keyOption1);

        if (!TryGet(getter, key1, out var keyValue1))
        {
            console.Error.WriteLine(FailureToFindMessage(key1, typeof(TValue1)));
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
                console.Error.WriteLine(DoesNotContainMessage(key1, key2.ToString()));
                continue;
            }

            removeAction(key1, key2);
            console.WriteLine($"{key2} successfully removed from {key1}");
        }
    }

    public static void DeleteHandler<TKey, TValue>(InvocationContext context, IConsole console,
        Action<TKey> deleteAction,
        IGetter<TValue, TKey> getter, Option<TKey> keyOption)
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (!TryGet(getter, key, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key, typeof(TValue)));
            return;
        }

        deleteAction(key);
        console.WriteLine(SuccessMessage(key, typeof(TValue), Operations.Deleted));
    }


    public static void ListHandler<TValue, _>(IConsole console, IGetter<TValue, _> getter, string header)
        where TValue : IListable
    {
        console.WriteLine(header);
        getter.GetAll().ToList().ForEach(e => console.WriteLine(e.ToListing()));
    }

    public static void ShowHandler<TKey, TValue>(InvocationContext context, IConsole console,
        IGetter<TValue, TKey> getter,
        Option<TKey> keyOption)
        where TValue : IListable
    {
        var key = GetValueOfRequiredOption(context, keyOption);

        if (!TryGet(getter, key, out var keyValue))
        {
            console.Error.WriteLine(FailureToFindMessage(key, typeof(TValue)));
            return;
        }

        console.WriteLine(keyValue.ToListing());
    }

    public static void ShowHandler<TKey1, TValue1, TKey2, TValue2, TValue3>(InvocationContext context, IConsole console,
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
            console.Error.WriteLine(FailureToFindMessage(key1, typeof(TValue1)));
            return;
        }

        if (!TryGet(getter2, key2, out _))
        {
            console.Error.WriteLine(FailureToFindMessage(key2, typeof(TValue2)));
            return;
        }

        console.WriteLine(getCurrent(key1, key2).ToListing());
    }

    public static void ExecuteHandlerList<TKey, TValue>(InvocationContext context, IConsole console,
        Action<TKey> executeAction,
        IGetter<TValue, TKey> getter,
        Option<IEnumerable<TKey>> keyOption)
    {
        if (!TryGetValueOfOption(context, keyOption, out var keyList))
        {
            return;
        }

        foreach (var key in keyList)
        {
            if (!TryGet(getter, key, out _))
            {
                console.Error.WriteLine(FailureToFindMessage(key, typeof(TValue)));
                continue;
            }

            executeAction(key);
            console.WriteLine(SuccessMessage(key, typeof(TValue), Operations.Executed));
        }
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
    {
        value = getter.Get(key);
        return value is not null;
    }

    private static string FailureToFindMessage<TKey>(TKey key, Type type)
    {
        return $"Could not find {GetEntityType(type)} using {key}";
    }

    private static string AlreadyExistsMessage<TKey>(TKey key, Type type)
    {
        return $"Found an existing {GetEntityType(type)} using {key}";
    }

    private static string SuccessMessage<TKey>(TKey key, Type type, Operations operation, params string[] parameters)
    {
        return SuccessMessage(key, type, OperationToString(operation), parameters);
    }

    private static string SuccessMessage<TKey>(TKey key, Type type, string operation, params string[] parameters)
    {
        return $"Successfully {operation} {key} {GetEntityType(type)}" +
               (parameters.Length != 0 ? $" with {string.Join(", ", parameters)}" : "");
    }

    private static string DoesNotContainMessage<TKey>(TKey key, string offender)
    {
        return $"{key} does not have {offender}, skipping";
    }

    private static string OperationToString(Operations operation)
    {
        return operation.ToString().ToLower();
    }

    private static string GetEntityType(Type type)
    {
        return type switch
        {
            not null when type == typeof(IDeleteCondition) => "delete condition",
            not null when type == typeof(IIndividual) => "individual",
            not null when type == typeof(IOrigin) => "origin",
            not null when type == typeof(IPersonalDataColumn) => "personal data column",
            not null when type == typeof(IProcessing) => "processing",
            not null when type == typeof(IPurpose) => "purpose",
            not null when type == typeof(IVacuumingRule) => "vacuuming rule",
            not null when type == typeof(DeleteCondition) => "delete condition",
            not null when type == typeof(Individual) => "individual",
            not null when type == typeof(Origin) => "origin",
            not null when type == typeof(PersonalData) => "personal data",
            not null when type == typeof(PersonalDataColumn) => "personal data column",
            not null when type == typeof(Processing) => "processing",
            not null when type == typeof(Purpose) => "purpose",
            not null when type == typeof(VacuumingRule) => "vacuuming rule",
            _ => "entity"
        };
    }

    private enum Operations
    {
        Updated,
        Deleted,
        Added,
        Set,
        Removed,
        Executed
    }
}