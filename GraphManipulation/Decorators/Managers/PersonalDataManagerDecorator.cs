using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Decorators.Managers;

public class PersonalDataManagerDecorator : LoggingDecorator, IPersonalDataManager
{

    private readonly IPersonalDataManager _manager;
    public PersonalDataManagerDecorator(IPersonalDataManager manager, ILogger logger) : base(logger, "personal data")
    {
        _manager = manager;
    }

    public IEnumerable<IPersonalDataColumn> GetAll()
    {
        return _manager.GetAll();
    }

    public IPersonalDataColumn? Get(TableColumnPair key)
    {
        return _manager.Get(key);
    }

    public void Delete(TableColumnPair key)
    {
        LogDelete(key.ToListing());
        _manager.Delete(key);
    }

    public void UpdateDescription(TableColumnPair key, string description)
    {
        LogUpdate(key.ToListing(), new {Description = description});
        _manager.UpdateDescription(key, description);
    }

    public void AddPersonalData(TableColumnPair tableColumnPair, string joinCondition, string description)
    {
        LogAdd(tableColumnPair.ToListing(), new {JoinCondition = joinCondition, Description = description});
        _manager.AddPersonalData(tableColumnPair, joinCondition, description);
    }

    public void SetDefaultValue(TableColumnPair tableColumnPair, string defaultValue)
    {
        throw new NotImplementedException();
    }

    public void AddPurpose(TableColumnPair tableColumnPair, string purposeName)
    {
        LogUpdate(tableColumnPair.ToListing(), new {PurposeName = purposeName});
        _manager.AddPurpose(tableColumnPair, purposeName);
    }

    public void RemovePurpose(TableColumnPair tableColumnPair, string purposeName)
    {
        LogUpdate(tableColumnPair.ToListing(), new {PurposeName = purposeName});
        _manager.RemovePurpose(tableColumnPair, purposeName);
    }

    public void SetOriginOf(TableColumnPair tableColumnPair, int individualsId, string originName)
    {
        LogUpdate(tableColumnPair.ToListing(), new {IndividualId = individualsId, Origin = originName});
        _manager.SetOriginOf(tableColumnPair, individualsId, originName);
    }

    public IOrigin GetOriginOf(TableColumnPair tableColumnPair, int individualsId)
    {
        return _manager.GetOriginOf(tableColumnPair, individualsId);
    }
}