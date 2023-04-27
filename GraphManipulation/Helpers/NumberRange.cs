using System.Collections;

namespace GraphManipulation.Helpers;

public class NumberRange : IEnumerable<int>
{
    public readonly int End;
    public readonly int Start;

    public NumberRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    public bool NumberWithinRange(int number)
    {
        return number >= Start && number <= End;
    }

    public bool Equals(NumberRange other)
    {
        return other.Start == Start && other.End == End;
    }

    public IEnumerator<int> GetEnumerator()
    {
        return new List<int> { Start, End }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}