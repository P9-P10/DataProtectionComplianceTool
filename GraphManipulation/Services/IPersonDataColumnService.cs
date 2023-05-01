using GraphManipulation.Models;

namespace GraphManipulation.Services;

public interface IPersonDataColumnService
{
    IEnumerable<PersonalDataColumn> GetColumns();
}