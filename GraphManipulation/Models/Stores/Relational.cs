using GraphManipulation.Models.QueryResults;

namespace GraphManipulation.Models.Stores;

public abstract class Relational : Database
{
    protected Relational(string name) : base(name)
    {
    }

    protected List<RelationalDatabaseStructureQueryResult> StructureQueryResults = new();

    protected List<RelationDatabaseForeignKeysQueryResult> ForeignKeysQueryResults = new();

    public override void Build()
    {
        base.Build();
        
        GetStructureQueryResults();
        GetForeignKeysQueryResults();
        BuildStructure();
        BuildForeignKeys();
    }
    
    protected abstract void GetStructureQueryResults();

    protected abstract void GetForeignKeysQueryResults();

    protected abstract void BuildStructure();

    protected abstract void BuildForeignKeys();
}