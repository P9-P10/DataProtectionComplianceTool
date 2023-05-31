﻿namespace GraphManipulation.Models;

public class PersonalDataColumn : Entity<TableColumnPair>
{
    public virtual IEnumerable<Purpose>? Purposes { get; set; }
    public string? DefaultValue { get; set; }
    public string? JoinCondition { get; set; }

    public override string ToListing()
    {
        return string.Join(ToListingSeparator, base.ToListing(),
            NullToString(DefaultValue),
            NullToString(JoinCondition),
            ListNullOrEmptyToString(Purposes));
    }

    public override string ToListingHeader()
    {
        return string.Join(ToListingSeparator, base.ToListingHeader(), "Default Value", "Join Condition", "Purposes");
    }
}