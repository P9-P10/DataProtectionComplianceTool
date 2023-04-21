using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class VacuumingRule<T> : IModel<T>
{
<<<<<<< HEAD
    public T Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
    public string Rule { get; private set; }
=======
    public IEnumerable<Purpose> Purposes { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Interval { get; set; }
    public string ToListing()
    {
        return string.Join(", ", Name, Description, Interval, "[ " + string.Join(", ", Purposes.Select(p => p.ToListing())) + " ]");
    }

    public string GetInterval()
    {
        return Interval;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description ?? "";
    }
>>>>>>> cli-2.0
}