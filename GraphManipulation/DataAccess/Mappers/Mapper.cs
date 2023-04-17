﻿namespace GraphManipulation.DataAccess.Mappers;

public class Mapper<T> : IMapper<T> where T : class
{
    private readonly GdprMetadataContext _context;

    public Mapper(GdprMetadataContext context)
    {
        _context = context;
    }
    
    public T Insert(T value)
    {
        ValidateArgument(value);
        
        var insertedValue = _context.Add(value).Entity;
        _context.SaveChanges();
        return insertedValue;
    }

    public IEnumerable<T> Find(Func<T, bool> condition)
    {
        return _context.Set<T>().Where(condition);
    }

    public T? FindSingle(Func<T, bool> condition)
    {
        return _context.Set<T>().SingleOrDefault(condition);
    }

    public T Update(T value)
    {
        ValidateArgument(value);
        var updatedValue = _context.Update(value).Entity;
        _context.SaveChanges();
        return updatedValue;
    }

    public void Delete(T value)
    {
        ValidateArgument(value);
        _context.Remove(value);
        _context.SaveChanges();
    }

    private static void ValidateArgument(T value)
    {
        // Check that the given argument is not null
        // Throw ArgumentNullException if it is
        if (value is null)
            throw new ArgumentNullException($"Expected argument 'value' to be of type {typeof(T)} but received null");
    }
}