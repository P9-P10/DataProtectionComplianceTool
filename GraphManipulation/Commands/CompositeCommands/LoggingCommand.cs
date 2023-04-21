using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Commands.BaseCommands;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;

namespace GraphManipulation.Commands.CompositeCommands;

public class LoggingCommand : AliasedCommand
{
    public LoggingCommand(ILogger logger, string? description = null)
        : base("log", "lg", description)
    {
        AddCommand(new ShowLogCommand(logger));
    }

    private sealed class ShowLogCommand : ShowCommand
    {
        public ShowLogCommand(ILogger logger, string? description = null) : base(description)
        {
            var numbersOption = CreateNumbersOption();
            var dateTimesOption = CreateDateTimeOption();
            var logTypesOption = CreateLogTypesOption();
            var logFormatsOption = CreateLogFormatOptions();
            
            AddOption(numbersOption);
            AddOption(dateTimesOption);
            AddOption(logTypesOption);
            AddOption(logFormatsOption);

            AddValidator(result => OptionBuilder.ValidateOrder(result, numbersOption));
            AddValidator(result => OptionBuilder.ValidateOrder(result, dateTimesOption));

            this.SetHandler((
                numbers,
                dateTimes,
                logTypes,
                messageFormats
            ) =>
            {
                var constraints = new LogConstraints(
                    new NumberRange(numbers.First(), numbers.Skip(1).First()),
                    new TimeRange(dateTimes.First(), dateTimes.Skip(1).First()),
                    logTypes.ToList(), messageFormats.ToList());

                var result = logger.Read(constraints);
                Console.WriteLine(result);
            }, numbersOption, dateTimesOption, logTypesOption, logFormatsOption);
        }

        private Option<IEnumerable<int>> CreateNumbersOption()
        {
            var exactlyTwo = new ArgumentArity(2, 2);
            
            return OptionBuilder.CreateOption<IEnumerable<int>>(name: "--numbers")
                .WithAlias("-n")
                .WithDescription("Limits results to the specified numbers range (inclusive).\n" +
                                 "Must provide two numbers as range (e.g. -n 3 6), first minimum then maximum")
                .WithArity(exactlyTwo)
                .WithAllowMultipleArguments(true)
                .WithGetDefaultValue(() => new List<int> { int.MinValue, int.MaxValue });
        }

        private Option<IEnumerable<DateTime>> CreateDateTimeOption()
        {
            var exactlyTwo = new ArgumentArity(2, 2);
            return OptionBuilder.CreateOption<IEnumerable<DateTime>>(name: "--date-times")
                .WithAlias("-d")
                .WithDescription("Limits results to the specified time range (inclusive).\n" +
                                 "Must provide two dates as range (e.g. -d 2000/04/28T12:34:56 3000/06/16T09:38:12), first minimum then maximum")
                .WithArity(exactlyTwo)
                .WithAllowMultipleArguments(true)
                .WithGetDefaultValue(() => new List<DateTime> { DateTime.MinValue, DateTime.MaxValue });
        }

        private Option<IEnumerable<LogType>> CreateLogTypesOption()
        {
            return OptionBuilder.CreateOption<IEnumerable<LogType>>("--log-types")
                .WithAlias("-lt")
                .WithDescription("Limits results to the specified log type(s).")
                .WithArity(ArgumentArity.OneOrMore)
                .WithAllowMultipleArguments(true)
                .WithGetDefaultValue(Enum.GetValues<LogType>)
                .FromAmong(Enum.GetNames<LogType>());
        }

        private Option<IEnumerable<LogMessageFormat>> CreateLogFormatOptions()
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
}