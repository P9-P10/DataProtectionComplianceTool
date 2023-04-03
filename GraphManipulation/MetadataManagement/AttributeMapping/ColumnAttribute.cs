namespace GraphManipulation.MetadataManagement.AttributeMapping;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public ColumnAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}