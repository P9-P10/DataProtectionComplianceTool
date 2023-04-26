using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;

namespace GraphManipulation.Commands.Builders;

public static class LoggingCommandBuilder
{
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
            .WithValidator(result => OptionBuilder.ValidateOrder(result, numbersOption))
            .WithValidator(result => OptionBuilder.ValidateOrder(result, dateTimesOption))
            .WithHandler(context =>
            {
                var numbers = context.ParseResult.GetValueForOption(numbersOption)!.ToList();
                var dateTimes = context.ParseResult.GetValueForOption(dateTimesOption)!.ToList();
                var logTypes = context.ParseResult.GetValueForOption(logTypesOption)!;
                var messageFormats = context.ParseResult.GetValueForOption(logFormatsOption)!;
                
                var constraints = new LogConstraints(
                    new NumberRange(numbers.First(), numbers.Skip(1).First()),
                    new TimeRange(dateTimes.First(), dateTimes.Skip(1).First()),
                    logTypes.ToList(), messageFormats.ToList());

                var result = logger.Read(constraints);
                console.Write(string.Join("\n", result));
            });
    }

    private static ArgumentArity ExactlyTwo => new(2, 2);

    private static Option<IEnumerable<int>> CreateNumbersOption()
    {
        return OptionBuilder.CreateOption<IEnumerable<int>>("--numbers")
            .WithAlias("-n")
            .WithDescription("Limits results to the specified numbers range (inclusive).\n" +
                             "Must provide two numbers as range (e.g. -n 3 6), first minimum then maximum")
            .WithArity(ExactlyTwo)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(() => new List<int> { int.MinValue, int.MaxValue });
    }

    private static Option<IEnumerable<DateTime>> CreateDateTimeOption()
    {
        return OptionBuilder.CreateOption<IEnumerable<DateTime>>("--date-times")
            .WithAlias("-d")
            .WithDescription("Limits results to the specified time range (inclusive).\n" +
                             "Must provide two dates as range (e.g. -d 2000/04/28T12:34:56 3000/06/16T09:38:12), first minimum then maximum")
            .WithArity(ExactlyTwo)
            .WithAllowMultipleArguments(true)
            .WithGetDefaultValue(() => new List<DateTime> { DateTime.MinValue, DateTime.MaxValue });
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