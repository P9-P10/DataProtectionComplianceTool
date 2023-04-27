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
            .WithOption(out var numbersOption, CreateNumbersOption())
            .WithOption(out var dateTimesOption, CreateDateTimeOption())
            .WithOption(out var logTypesOption, CreateLogTypesOption())
            .WithOption(out var logFormatsOption, CreateLogFormatOptions())
            .WithValidator(result => OptionBuilder.ValidateOrder<NumberRange, int>(result, numbersOption))
            .WithValidator(result => OptionBuilder.ValidateOrder<TimeRange, DateTime>(result, dateTimesOption))
            .WithHandler(context =>
            {
                var numbers = context.ParseResult.GetValueForOption(numbersOption)!;
                var dateTimes = context.ParseResult.GetValueForOption(dateTimesOption)!;
                var logTypes = context.ParseResult.GetValueForOption(logTypesOption)!;
                var messageFormats = context.ParseResult.GetValueForOption(logFormatsOption)!;

                var constraints = new LogConstraints(numbers, dateTimes, logTypes.ToList(), messageFormats.ToList());

                var result = logger.Read(constraints);
                console.Write(string.Join("\n", result));
            });
    }

    private static Option<NumberRange> CreateNumbersOption()
    {
        return new Option<NumberRange>(
                "--numbers",
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
                            $"--numbers require input to be integers, which \"{startString} {endString}\" is not";
                    }

                    result.ErrorMessage = "--numbers requires two arguments";
                    return new NumberRange(0, 0);
                })
            .WithAlias("-n")
            .WithDescription("Limits results to the specified numbers range (inclusive).\n" +
                             "Must provide two numbers as range (e.g. -n 3 6), first minimum then maximum")
            .WithArity(ExactlyTwo)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(() => new NumberRange(int.MinValue, int.MaxValue));
    }

    private static Option<TimeRange> CreateDateTimeOption()
    {
        return new Option<TimeRange>(
                "--date-times",
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
                            $"--date-times require input to be date times, which \"{startString} {endString}\" is not";
                    }

                    result.ErrorMessage = "--numbers requires two arguments";
                    return new TimeRange(DateTime.Now, DateTime.Now);
                })
            .WithAlias("-d")
            .WithDescription("Limits results to the specified time range (inclusive).\n" +
                             "Must provide two date times as range (e.g. -d 2000/04/28T12:34:56 3000/06/16T09:38:12), first minimum then maximum")
            .WithArity(ExactlyTwo)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(() => new TimeRange(DateTime.MinValue, DateTime.MaxValue));
    }

    private static Option<IEnumerable<LogType>> CreateLogTypesOption()
    {
        return OptionBuilder.CreateOption<IEnumerable<LogType>>("--log-types")
            .WithAlias("-lt")
            .WithDescription("Limits results to the specified log type(s).")
            .WithArity(ArgumentArity.OneOrMore)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(Enum.GetValues<LogType>)
            .FromAmong(Enum.GetNames<LogType>());
    }

    private static Option<IEnumerable<LogMessageFormat>> CreateLogFormatOptions()
    {
        return OptionBuilder.CreateOption<IEnumerable<LogMessageFormat>>("--log-formats")
            .WithAlias("-lf")
            .WithDescription("Limits results to the specified log format(s)")
            .WithArity(ArgumentArity.OneOrMore)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(Enum.GetValues<LogMessageFormat>)
            .FromAmong(Enum.GetNames<LogMessageFormat>());
    }
}