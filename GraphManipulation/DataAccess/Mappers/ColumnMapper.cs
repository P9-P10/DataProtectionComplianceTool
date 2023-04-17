using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Mappers;

public class ColumnMapper : IMapper<PersonDataColumn>
{
    public PersonDataColumn Insert(PersonDataColumn value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PersonDataColumn> Find(Func<PersonDataColumn, bool> condition)
    {
        throw new NotImplementedException();
    }

    public PersonDataColumn FindSingle(Func<PersonDataColumn, bool> condition)
    {
        throw new NotImplementedException();
    }

    public PersonDataColumn Update(PersonDataColumn value)
    {
        throw new NotImplementedException();
    }

    public void Delete(PersonDataColumn value)
    {
        throw new NotImplementedException();
    }
}