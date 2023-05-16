using GraphManipulation.DataAccess;
using GraphManipulation.Decorators;
using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.Factories;

public class VacuumerFactory : IVacuumerFactory
{
    private readonly IMapper<Purpose>? _mapper;
    private readonly IQueryExecutor? _queryExecutor;
    private readonly IVacuumer? _vacuumer;
    
    public VacuumerFactory(IVacuumer vacuumer)
    {
        _vacuumer = vacuumer;
    }

    public VacuumerFactory(IMapper<Purpose> mapper, IQueryExecutor queryExecutor)
    {
        _mapper = mapper;
        _queryExecutor = queryExecutor;
    }
    
    public IVacuumer CreateVacuumer()
    {
        return _vacuumer ?? new Vacuumer(_mapper, _queryExecutor);
    }
}

public class LoggingVacuumerFactory : IVacuumerFactory
{
    private readonly IVacuumerFactory _vacuumerFactory;
    private readonly ILogger _logger;

    public LoggingVacuumerFactory(IVacuumerFactory vacuumerFactory, ILogger logger)
    {
        _vacuumerFactory = vacuumerFactory;
        _logger = logger;
    }

    public IVacuumer CreateVacuumer()
    {
        return new LoggingVacuumer(_vacuumerFactory.CreateVacuumer(), _logger);
    }
}