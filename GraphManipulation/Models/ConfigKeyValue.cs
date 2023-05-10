using GraphManipulation.Models.Base;

namespace GraphManipulation.Models;

public class ConfigKeyValue : DomainEntity
{
    public string Key { get; set; }
    public string Value { get; set; }
}