using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Mappers;

public class OriginMapper : IMapper<Origin>
{
    private readonly GdprMetadataContext _context;

    public OriginMapper(GdprMetadataContext context)
    {
        _context = context;
    }
    public Origin Insert(Origin value)
    {
        var insertedValue = _context.origins.Add(value).Entity;
        _context.SaveChanges();
        return insertedValue;
    }

    public IEnumerable<Origin> Find(Func<Origin, bool> condition)
    {
        return _context.origins.Where(condition); 
    }

    public Origin? FindSingle(Func<Origin, bool> condition)
    {
        return _context.origins.SingleOrDefault(condition);
    }

    public Origin Update(Origin value)
    {
        throw new NotImplementedException();
    }

    public void Delete(Origin value)
    {
        throw new NotImplementedException();
    }
}