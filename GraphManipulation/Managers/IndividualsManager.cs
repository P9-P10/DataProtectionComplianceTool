using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class IndividualsManager : IIndividualsManager
{
    private IMapper<Individual> _mapper;
    private TableColumnPair _individualsSource;

    public IndividualsManager(IMapper<Individual> individualsMapper)
    {
        _mapper = individualsMapper;
    }
    public IEnumerable<IIndividual> GetAll()
    {
        return _mapper.Find(_ => true);
    }

    public IIndividual? Get(int key)
    {
        return _mapper.FindSingle(individual => individual.Id == key);
    }

    public void SetIndividualsSource(TableColumnPair source)
    {
        _individualsSource = source;
    }

    public TableColumnPair GetIndividualsSource()
    {
        return _individualsSource;
    }
}