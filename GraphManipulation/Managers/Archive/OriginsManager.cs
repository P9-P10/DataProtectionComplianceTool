using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
using IOriginsManager = GraphManipulation.Managers.Interfaces.Archive.IOriginsManager;

namespace GraphManipulation.Managers.Archive;

public class OriginsManager : NamedEntityManager<Origin>, IOriginsManager
{
    private IMapper<Origin> _originMapper;

    public OriginsManager(IMapper<Origin> originMapper) : base(originMapper)
    {
        _originMapper = originMapper;
    }

    public IEnumerable<IOrigin> GetAll() => base.GetAll();
    public IOrigin? Get(string key) => base.Get(key);
    public void UpdateName(string name, string newName) => base.UpdateName(name, newName);

    public void UpdateDescription(string key, string description)
    {
        var origin = GetByName(key);
        origin.Description = description;
        _originMapper.Update(origin);
    }

    public void Add(string name, string description)
    {
        _originMapper.Insert(new Origin { Name = name, Description = description });
    }
}