using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers.Interfaces.Archive;

public interface IPurposesManager : 
    IGetter<IPurpose, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public void Add(string name, bool legallyRequired, string description);
    public void UpdateLegallyRequired(string name, bool legallyRequired);

    public void SetDeleteCondition(string purposeName, string deleteConditionName);
}