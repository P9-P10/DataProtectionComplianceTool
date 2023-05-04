namespace GraphManipulation.Commands.Helpers;

public static class CommandHeader
{
    public const string OriginsHeader = "TableName, ColumnName, JoinCondition, Description, Purposes";

    public const string VacuumingHeader = "Name, Description, Interval, Purposes";

    public const string ProcessingsHeader = "Name, Description, Purposes, Table, Personal Data Columns";
    
    public const string PurposesHeader = "Name, Description, Legally Required, Deletion Condition, Personal Data Columns, Rules";

    public const string IndividualsHeader = "Id";

    public const string PersonalDataHeader = "Table, Column, Join Condition, Description, Default Value, Purposes";
}