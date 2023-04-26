namespace GraphManipulation.Helpers;

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

    public bool Equals(TimeRange other)
    {
        return other.Start == Start && other.End == End;
    }
}