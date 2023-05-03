using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class ProcessingsManager : NamedEntityManager<Processing>, IProcessingsManager
{
    private IMapper<Processing> _processingMapper;
    private IMapper<Purpose> _purposeMapper;
    private IMapper<PersonalDataColumn> _columnMapper;
    public ProcessingsManager(IMapper<Processing> mapper, IMapper<Purpose> purposeMapper, IMapper<PersonalDataColumn> columnMapper) : base(mapper)
    {
        _processingMapper = mapper;
        _purposeMapper = purposeMapper;
        _columnMapper = columnMapper;
    }

    public IEnumerable<IProcessing> GetAll() => base.GetAll();
    public IProcessing? Get(string key)
    {
        return base.Get(key);
    }

    public void UpdateDescription(string key, string description)
    {
        var processing = GetByName(key);
        processing.Description = description;
        _processingMapper.Update(processing);
    }
    
    public void AddProcessing(string name, TableColumnPair tableColumnPair, string purposeName, string description)
    {
        var purpose = _purposeMapper.FindSingle(purpose => purpose.Name == purposeName);
        var column = _columnMapper.FindSingle(column => column.TableColumnPair.Equals(tableColumnPair));
        _processingMapper.Insert(new Processing { Name = name, Description = description, Purpose = purpose, PersonalDataColumn = column});
    }
}