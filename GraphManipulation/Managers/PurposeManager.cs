using System.Runtime.InteropServices.ComTypes;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class PurposeManager : IPurposesManager
{
    private IMapper<Purpose> _purposeMapper;
    private IMapper<DeleteCondition> _conditionsManager;
    
    public PurposeManager(IMapper<Purpose> purposeMapper, IMapper<DeleteCondition> conditionsMapper)
    {
        _purposeMapper = purposeMapper;
        _conditionsManager = conditionsMapper;
    }
    
    public IEnumerable<IPurpose> GetAll()
    {
        return _purposeMapper.Find(_ => true);
    }

    public IPurpose? Get(string key)
    {
        return GetByName(key);
    }

    public void Delete(string key)
    {
        _purposeMapper.Delete(GetByName(key));
    }

    public void UpdateName(string name, string newName)
    {
        var purpose = GetByName(name);
        purpose.Name = newName;
        _purposeMapper.Update(purpose);
    }

    public void UpdateDescription(string key, string description)
    {
        var purpose = GetByName(key);
        purpose.Description = description;
        _purposeMapper.Update(purpose);
    }

    public void Add(string name, bool legallyRequired, string description)
    {
        _purposeMapper.Insert(new Purpose { Name = name,LegallyRequired = legallyRequired, Description = description });
    }

    public void UpdateLegallyRequired(string name, bool legallyRequired)
    {
        var purpose = GetByName(name);
        purpose.LegallyRequired = legallyRequired;
        _purposeMapper.Update(purpose);
    }

    public void SetDeleteCondition(string purposeName, string deleteConditionName)
    {
        var purpose = GetByName(purposeName);
        purpose.DeleteCondition = _conditionsManager.FindSingle(condition => condition.Name == deleteConditionName);
        _purposeMapper.Update(purpose);
    }

    private Purpose? GetByName(string name)
    {
        return _purposeMapper.FindSingle(purpose => purpose.Name == name);
    }
}