using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;

namespace GraphManipulation.Managers.Interfaces;

public interface IPurposeManager : 
    IGetter<Purpose, string>, 
    IDeleter<string>, 
    INameUpdater, 
    IDescriptionUpdater<string>
{
    public void Add(string name, bool legallyRequired, string description);
    public void UpdateLegallyRequired(string name, bool legallyRequired);

    public void SetDeleteCondition(string purposeName, string deleteConditionName);
}