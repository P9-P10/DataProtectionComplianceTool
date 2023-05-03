using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class IndividualsManager : IIndividualsManager
{
    private readonly IMapper<Individual> _mapper;
    private readonly IMapper<ConfigKeyValue> _keyValueMapper;
    private const string IndividualsSourceKey = "IndividualsSource";

    public IndividualsManager(IMapper<Individual> individualsMapper, IMapper<ConfigKeyValue> keyValueMapper)
    {
        _mapper = individualsMapper;
        _keyValueMapper = keyValueMapper;
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
        ConfigKeyValue keyValue = _keyValueMapper.FindSingle(x => x.Key == IndividualsSourceKey);
        if (keyValue == null)
        {
            _keyValueMapper.Insert(new ConfigKeyValue()
                {Key = IndividualsSourceKey, Value = $"({source.TableName},{source.ColumnName})"});
        }
        else
        {
            keyValue.Value = $"({source.TableName},{source.ColumnName})";
            _keyValueMapper.Update(keyValue);
        }
    }

    public TableColumnPair GetIndividualsSource()
    {
        ConfigKeyValue? keyValue = _keyValueMapper.FindSingle(x => x.Key == IndividualsSourceKey);
        string table = keyValue.Value.Split(",")[0].Replace("(", "");
        string column = keyValue.Value.Split(",")[1].Replace(")", "");
        TableColumnPair tableColumnPair = new TableColumnPair(table, column);
        return tableColumnPair;
    }
}