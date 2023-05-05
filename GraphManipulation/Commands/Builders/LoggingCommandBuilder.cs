using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;

namespace GraphManipulation.Commands.Builders;

public static class LoggingCommandBuilder
{
    private static ArgumentArity ExactlyTwo => new(2, 2);

    public static Command Build(IConsole console, ILogger logger)
    {
        return CommandBuilder.CreateCommand(CommandNamer.LoggingName)
            .WithAlias(CommandNamer.LoggingAlias)
            .WithSubCommands(ListLog(console, logger));
    }

    private static Command ListLog(IConsole console, ILogger logger)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the logs that fall within the given constraints")
            .WithOption(out var limitOption, CreateLimitOption())
            .WithOption(out var numbersOption, CreateNumbersOption())
            .WithOption(out var dateTimesOption, CreateDateTimeOption())
            .WithOption(out var logTypesOption, CreateLogTypesOption())
            .WithOption(out var subjectsOption, CreateSubjectsOption())
            .WithOption(out var logFormatsOption, CreateLogFormatOptions())
            .WithValidator(result => OptionBuilder.ValidateOrder<NumberRange, int>(result, numbersOption))
            .WithValidator(result => OptionBuilder.ValidateOrder<TimeRange, DateTime>(result, dateTimesOption))
            .WithHandler(context =>
            {
                var limit = context.ParseResult.GetValueForOption(limitOption);
                var numbers = context.ParseResult.GetValueForOption(numbersOption)!;
                var dateTimes = context.ParseResult.GetValueForOption(dateTimesOption)!;
                var logTypes = context.ParseResult.GetValueForOption(logTypesOption)!;
                var subjects = context.ParseResult.GetValueForOption(subjectsOption)!;
                var messageFormats = context.ParseResult.GetValueForOption(logFormatsOption)!;

                var constraints = new LogConstraints(numbers, dateTimes, logTypes.ToList(), subjects,
                    messageFormats.ToList(), limit);

                var result = logger.Read(constraints);
                console.Write(string.Join("\n", result));
            });
    }

    private static Option<NumberRange> CreateNumbersOption()
    {
        return new Option<NumberRange>(
                OptionNamer.Numbers,
                result =>
                {
                    if (result.Tokens.Count == 2)
                    {
                        var startString = result.Tokens[0].Value;
                        var endString = result.Tokens[1].Value;

                        if (int.TryParse(startString, out var start) &&
                            int.TryParse(endString, out var end))
                        {
                            return new NumberRange(start, end);
                        }

                        result.ErrorMessage =
                            $"{OptionNamer.Numbers} require input to be integers, which \"{startString} {endString}\" is not";
                    }

                    result.ErrorMessage = $"{OptionNamer.Numbers} requires two arguments";
                    return new NumberRange(0, 0);
                })
            .WithAlias(OptionNamer.NumbersAlias)
            .WithDescription("Limits results to the specified numbers range (inclusive). " +
                             $"Must provide two numbers as range (e.g. {OptionNamer.NumbersAlias} 3 6), first minimum then maximum")
            .WithArity(ExactlyTwo)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(() => new NumberRange(0, int.MaxValue));
    }

    private static Option<TimeRange> CreateDateTimeOption()
    {
        return new Option<TimeRange>(
                OptionNamer.DateTimes,
                result =>
                {
                    if (result.Tokens.Count == 2)
                    {
                        var startString = result.Tokens[0].Value;
                        var endString = result.Tokens[1].Value;

                        if (DateTime.TryParse(startString, out var start) &&
                            DateTime.TryParse(endString, out var end))
                        {
                            return new TimeRange(start, end);
                        }

                        result.ErrorMessage =
                            $"{OptionNamer.DateTimes} require input to be date times, which \"{startString} {endString}\" is not";
                    }

                    result.ErrorMessage = "--numbers requires two arguments";
                    return new TimeRange(DateTime.Now, DateTime.Now);
                })
            .WithAlias(OptionNamer.DateTimesAlias)
            .WithDescription("Limits results to the specified time range (inclusive). " +
                             $"Must provide two date times as range (e.g. {OptionNamer.DateTimesAlias} 2000/04/28T12:34:56 3000/06/16T09:38:12), first minimum then maximum")
            .WithArity(ExactlyTwo)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(() => new TimeRange(DateTime.MinValue, DateTime.MaxValue));
    }

    private static Option<IEnumerable<LogType>> CreateLogTypesOption()
    {
        return OptionBuilder.CreateOption<IEnumerable<LogType>>(OptionNamer.LogTypes)
            .WithAlias(OptionNamer.LogTypesAlias)
            .WithDescription("Limits results to the specified log type(s).")
            .WithArity(ArgumentArity.OneOrMore)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(Enum.GetValues<LogType>)
            .FromAmong(Enum.GetNames<LogType>());
    }

    private static Option<IEnumerable<string>> CreateSubjectsOption()
    {
        return OptionBuilder.CreateOption<IEnumerable<string>>(OptionNamer.Subjects)
            .WithAlias(OptionNamer.SubjectsAlias)
            .WithDescription("Limits results to the specified subject(s).")
            .WithArity(ArgumentArity.OneOrMore)
            .WithAllowMultipleArguments(true)
            .WithDefaultValue(Array.Empty<string>());
    }

    private static Option<IEnumerable<LogMessageFormat>> CreateLogFormatOptions()
    {
        return OptionBuilder.CreateOption<IEnumerable<LogMessageFormat>>(OptionNamer.LogFormats)
            .WithAlias(OptionNamer.LogFormatsAlias)
            .WithDescription("Limits results to the specified log format(s)")
            .WithArity(ArgumentArity.OneOrMore)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(Enum.GetValues<LogMessageFormat>)
            .FromAmong(Enum.GetNames<LogMessageFormat>());
    }

    private static Option<int> CreateLimitOption()
    {
        return OptionBuilder.CreateOption<int>(OptionNamer.Limit)
            .WithAlias(OptionNamer.LimitAlias)
            .WithDescription("Limits results to the number given")
            .WithDefaultValue(100);
    }
}