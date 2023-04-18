using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Mappers;

public class ColumnMapper : IMapper<Column>
{
    public Column Insert(Column value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Column> Find(Func<Column, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Column FindSingle(Func<Column, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Column Update(Column value)
    {
        throw new NotImplementedException();
    }

    public void Delete(Column value)
    {
        throw new NotImplementedException();
    }
}