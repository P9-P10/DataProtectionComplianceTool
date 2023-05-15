using GraphManipulation.Managers;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class PersonalDataColumn : Entity<TableColumnPair>
{
    public virtual IEnumerable<Purpose>? Purposes { get; set; }
    public string? DefaultValue { get; set; } = "";

    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(), DefaultValue, 
            "[ " + string.Join(", ", Purposes == null ? new List<string>() : Purposes.Select(p => p.ToListingIdentifier())) + " ]");
    }

    // public void AddPurpose(Purpose purpose)
    // {
    //     if (Purposes == null)
    //     {
    //         Purposes = new List<Purpose> { purpose };
    //     }
    //     else
    //     {
    //         var l = Purposes.ToList();
    //         l.Add(purpose);
    //         Purposes = l;
    //     }
    // }
}