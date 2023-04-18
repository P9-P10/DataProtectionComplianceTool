using GraphManipulation.Models;

namespace GraphManipulation.Managers;

public interface IPurposeManager
{
    public Purpose AddPurpose(string name, bool legallyRequired, string description);
    public Purpose UpdatePurposeName(string name, string newName);
    public Purpose UpdatePurposeLegallyRequired(string name, bool legallyRequired);
    public Purpose UpdatePurposeDescription(string name, string description);
    public void DeletePurpose(string name);
    public IEnumerable<Purpose> GetAllPurposes();
    public Purpose? GetPurpose(string name);

    public Purpose SetDeleteCondition(string purposeName, string deleteConditionName);
}