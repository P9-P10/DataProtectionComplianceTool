using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Mappers;

public class PurposeMapper : IMapper<Purpose>
{
    public Purpose Insert(Purpose value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Purpose> Find(Func<Purpose, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Purpose? FindSingle(Func<Purpose, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Purpose Update(Purpose value)
    {
        throw new NotImplementedException();
    }

    public void Delete(Purpose value)
    {
        throw new NotImplementedException();
    }
}