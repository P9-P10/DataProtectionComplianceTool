namespace GraphManipulation.Helpers;

public class NumberRange
{
    public readonly int Start;
    public readonly int End;

    public NumberRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    public bool NumberWithinRange(int number)
    {
        return number >= Start && number <= End;
    }
}