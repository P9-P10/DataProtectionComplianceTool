using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;

namespace GraphManipulation.Models.Structures;

public abstract class Structure : StructuredEntity //, IHasStructure
{
    public Structure? ParentStructure;
    public Database? Database;

    protected Structure(string name) : base(name)
    {
    }

    public override void UpdateBaseUri(string baseUri)
    {
        BaseUri = baseUri;

        if (!IsTop() && !ParentStructure!.HasSameBase(baseUri))
        {
            ParentStructure.UpdateBaseUri(baseUri);
        }

        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
                if (!subStructure.HasSameBase(baseUri))
                {
                    subStructure.UpdateBaseUri(baseUri);
                }
        }

        if (IsTop() && HasDatabase() && !Database!.HasSameBase(baseUri))
        {
            Database.UpdateBaseUri(baseUri);
        }

        ComputeId();
    }

    public void UpdateDatabase(Database database)
    {
        Database = database;

        if (!IsTop() && !ParentStructure!.HasSameDatabase(database))
        {
            ParentStructure.UpdateDatabase(database);
        }

        if (database.HasBase())
        {
            UpdateBaseUri(database.BaseUri);
        }

        ComputeId();

        if (IsBottom())
        {
            return;
        }

        foreach (var subStructure in SubStructures)
            if (!subStructure.HasSameDatabase(database))
            {
                subStructure.UpdateDatabase(database);
            }
    }

    public override void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure))
        {
            return;
        }

        if (!structure.IsTop())
        {
            structure.ParentStructure!.SubStructures.Remove(structure);
        }

        SubStructures.Add(structure);
        structure.ParentStructure = this;

        if (HasDatabase())
        {
            UpdateDatabase(Database!);
        }

        if (HasBase())
        {
            UpdateBaseUri(BaseUri!);
        }

        UpdateIdToBottom();
    }

    public bool IsTop()
    {
        return ParentStructure is null;
    }

    public bool HasDatabase()
    {
        return Database is not null;
    }

    public bool HasSameDatabase(Database database)
    {
        return HasDatabase() && Database!.Equals(database);
    }

    public override List<string> ConstructIdString()
    {
        var result = new List<string>();

        if (IsTop())
        {
            if (HasDatabase())
            {
                result = Database!.ConstructIdString();
            }
        }
        else
        {
            result = ParentStructure!.ConstructIdString();
        }

        result.Add(Name);

        return result;
    }
}

public class StructureException : Exception
{
    public StructureException(string message) : base(message)
    {
    }
}