using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Mappers;

public class PersonMapper : IMapper<Person>
{
    public Person Insert(Person value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Person> Find(Func<Person, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Person FindSingle(Func<Person, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Person Update(Person value)
    {
        throw new NotImplementedException();
    }

    public void Delete(Person value)
    {
        throw new NotImplementedException();
    }
}