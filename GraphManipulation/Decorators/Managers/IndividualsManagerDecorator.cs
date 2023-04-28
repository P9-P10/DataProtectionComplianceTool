using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Decorators.Managers;

public class IndividualsManagerDecorator : LoggingDecorator, IIndividualsManager
{
    private IIndividualsManager _manager;

    public IndividualsManagerDecorator(IIndividualsManager manager, ILogger logger) :
        base(logger, "individual")
    {
        _manager = manager;
    }


    public IEnumerable<IIndividual> GetAll()
    {
        return _manager.GetAll();
    }

    public IIndividual? Get(int key)
    {
        return _manager.Get(key);
    }

    public void SetIndividualsSource(TableColumnPair source)
    {
        LogAdd(source.ToListing(),source);
        _manager.SetIndividualsSource(source);
    }
}