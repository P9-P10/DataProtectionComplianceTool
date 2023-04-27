using System.Collections;

namespace GraphManipulation.Helpers;

public class TimeRange : IEnumerable<DateTime>
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


    public IEnumerator<DateTime> GetEnumerator()
    {
        return new List<DateTime> { Start, End }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}