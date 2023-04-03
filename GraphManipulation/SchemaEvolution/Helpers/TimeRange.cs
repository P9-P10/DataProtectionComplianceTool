namespace GraphManipulation.SchemaEvolution.Helpers;

public class TimeRange
{
    public readonly DateTime End;
    public readonly DateTime Start;

    public TimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public bool DateTimeWithinRange(DateTime dateTime)
    {
        return dateTime >= Start && dateTime <= End;
    }
}