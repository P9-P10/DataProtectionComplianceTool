using GraphManipulation.Managers;
using GraphManipulation.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GraphManipulation.Models;

public class Purpose<T> : IModel<T>
{
<<<<<<< HEAD
    public T Identifier { get; private set; }
    public string Description { get; private set; }
    public string Name { get; private set; }
=======
    public string? Description { get; set; }
    public string Name { get; set; }
    public bool LegallyRequired { get; set; }
    public IEnumerable<PersonalDataColumn> Columns { get; set; }
    public IEnumerable<VacuumingRule> Rules { get; set; }
    public DeleteCondition? DeleteCondition { get; set; }


    public string ToListing()
    {
        return string.Join(", ",
            Name,
            Description,
            LegallyRequired,
            DeleteCondition.ToListing(),
            "[ " + Columns.Select(c => c.ToListing()) + " ]",
            "[ " + Rules.Select(r => r.ToListing()) + " ]"
        );
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description ?? "";
    }

    public bool GetLegallyRequired()
    {
        return LegallyRequired;
    }

    public IDeleteCondition? GetDeleteCondition()
    {
        return DeleteCondition;
    }
>>>>>>> cli-2.0
}