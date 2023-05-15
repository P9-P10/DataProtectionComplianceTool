using GraphManipulation.Models;

namespace GraphManipulation.Helpers;

public static class TypeToString
{
    public static string GetEntityType(Type type)
    {
        return type switch
        {
            not null when type == typeof(StorageRule) => "delete condition",
            not null when type == typeof(Individual) => "individual",
            not null when type == typeof(Origin) => "origin",
            not null when type == typeof(PersonalDataOrigin) => "personal data origin",
            not null when type == typeof(PersonalDataColumn) => "personal data column",
            not null when type == typeof(Processing) => "processing",
            not null when type == typeof(Purpose) => "purpose",
            not null when type == typeof(VacuumingRule) => "vacuuming rule",
            _ => "entity"
        };
    }
}