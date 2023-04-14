using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Managers;

public class PurposeManager : IPurposeManager<int>
{
    public void Add(Purpose<int> value)
    {
        throw new NotImplementedException();
    }

    public void Update(int id, Purpose<int> value)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Purpose<int> Get(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose<int>> GetAll()
    {
        throw new NotImplementedException();
    }
}