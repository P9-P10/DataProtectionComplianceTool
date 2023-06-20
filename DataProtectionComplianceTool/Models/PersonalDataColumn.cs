namespace GraphManipulation.Models;

public class PersonalDataColumn : Entity<TableColumnPair>
{
    public virtual IEnumerable<Purpose>? Purposes { get; set; }
    public virtual IEnumerable<LegalBasis>? LegalBases { get; set; }
    public string? DefaultValue { get; set; }
    public string? AssociationExpression { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(DefaultValue),
            NullToString(AssociationExpression),
            ListNullOrEmptyToString(Purposes),
            ListNullOrEmptyToString(LegalBases));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Default Value", "Association Expression", "Purposes", "Legal bases");
    }
}