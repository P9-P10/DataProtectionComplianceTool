using GraphManipulation.DataAccess.Entities;

namespace GraphManipulation.Services;

public interface IPersonDataColumnService
{
    IEnumerable<PersonDataColumn> GetColumns();
}