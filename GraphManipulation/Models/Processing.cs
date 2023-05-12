using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class Processing : Entity<string>
{
    public virtual Purpose? Purpose { get; set; }
    public virtual PersonalDataColumn? PersonalDataColumn { get; set; }
    public override string ToListing()
    {
        return string.Join(", ", base.ToListing(), Purpose?.ToListingIdentifier(),
            PersonalDataColumn?.ToListingIdentifier());
    }
    
    // public override void Fill(object? other)
    // {
    //     if (other is null || other.GetType() != typeof(Processing))
    //     {
    //         return;
    //     }
    //     
    //     base.Fill(other);
    //
    //     var otherProcessing = (other as Processing)!;
    //     
    //     otherProcessing.Purpose ??= Purpose;
    //     otherProcessing.PersonalDataColumn ??= PersonalDataColumn;
    // }
}