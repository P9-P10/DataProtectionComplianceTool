using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class Origin : Entity<string>
{
    public virtual IEnumerable<PersonalDataColumn>? PersonalDataColumns { get; set; }

    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(), 
            ListNullOrEmptyToString(PersonalDataColumns));
    }

    // public override void Fill(object? other)
    // {
    //     if (other is null || other.GetType() != typeof(Origin))
    //     {
    //         return;
    //     }
    //     
    //     base.Fill(other);
    //     
    //     (other as Origin)!.PersonalDataColumns ??= PersonalDataColumns;
    // }
}