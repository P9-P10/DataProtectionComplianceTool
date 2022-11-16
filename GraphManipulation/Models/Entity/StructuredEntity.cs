using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Entity;

public abstract class StructuredEntity : NamedEntity
{
    public readonly List<Structure> SubStructures = new();

    protected StructuredEntity(string name) : base(name)
    {
    }
    
    public void UpdateIdToBottom()
    {
        ComputeId();

        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
                subStructure.UpdateIdToBottom();
        }
    }
    
    public bool IsBottom()
    {
        return SubStructures.Count == 0;
    }

    public override void UpdateName(string name)
    {
        base.UpdateName(name);
        UpdateIdToBottom();
    }

    public abstract void AddStructure(Structure structure);
}