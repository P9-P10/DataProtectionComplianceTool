using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Decorators;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;

namespace GraphManipulation.Commands.Factories;

public class ManagerFactory : IManagerFactory
{
    private GdprMetadataContext _dbContext;

    public ManagerFactory(GdprMetadataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull
    {
        return new Manager<TK, TV>(new Mapper<TV>(_dbContext));
    }
}

public class LoggingManagerFactory : IManagerFactory
{
    private IManagerFactory _managerFactory;
    private readonly ILogger _logger;

    public LoggingManagerFactory(IManagerFactory managerFactory, ILogger logger)
    {
        _managerFactory = managerFactory;
        _logger = logger;
    }

    public IManager<TK, TV> CreateManager<TK, TV>() where TV : Entity<TK>, new() where TK : notnull
    {
        return new LoggingManager<TK, TV>(_managerFactory.CreateManager<TK, TV>(), _logger);
    }
}