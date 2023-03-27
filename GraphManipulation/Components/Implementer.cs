using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Components;

public class Implementer
{
    public string Move1(IGraph schema, Table sourceRelation, Table destinationRelation, Column element,
        List<string> conditions)
    {
        var dropViews = @"
            DROP VIEW IF EXISTS MOVE_1_temp;
            DROP VIEW IF EXISTS MOVE_1;";

        var temp = $@"
            CREATE VIEW MOVE_1_temp AS
            SELECT {destinationRelation.Name}.*, {element.Name}
            FROM {destinationRelation.Name}
                     LEFT JOIN {sourceRelation.Name} on {string.Join(" and ", conditions)};";

        var final = $@"
            CREATE VIEW MOVE_1 AS
            SELECT M.*,
                   CASE WHEN t.id is null THEN FALSE ELSE TRUE END AS originally_in_{sourceRelation.Name}
            FROM MOVE_1_temp AS M LEFT JOIN (SELECT id from {sourceRelation.Name}) as t
            {string.Join(" and ", conditions)};

            SELECT * FROM MOVE_1;";

        return string.Join("\n", new List<string> { dropViews, temp, final });
    }
}