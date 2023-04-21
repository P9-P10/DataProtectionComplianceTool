using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models;

public class Origin<T> : IModel<T>
{
<<<<<<< HEAD
    public T Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; }
=======
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<PersonalDataColumn> PersonalDataColumns { get; set; }
    
    public string ToListing()
    {
        return string.Join(", ", Name, Description, "[ " + PersonalDataColumns.Select(c => c.ToListing()) + " ]");
    }

    public IEnumerable<IPersonalDataColumn> GetPersonalDataColumns()
    {
        return PersonalDataColumns;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }
>>>>>>> cli-2.0
}