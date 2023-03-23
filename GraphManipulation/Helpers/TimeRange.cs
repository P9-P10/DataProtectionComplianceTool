namespace GraphManipulation.Helpers;

public class TimeRange
{
    public readonly DateTime Start;
    public readonly DateTime End;
    
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