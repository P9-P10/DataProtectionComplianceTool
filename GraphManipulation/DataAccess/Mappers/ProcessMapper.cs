using GraphManipulation.Models;

namespace GraphManipulation.DataAccess.Mappers;

public class ProcessMapper : IMapper<Process>
{
    public Process Insert(Process value)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Process> Find(Func<Process, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Process FindSingle(Func<Process, bool> condition)
    {
        throw new NotImplementedException();
    }

    public Process Update(Process value)
    {
        throw new NotImplementedException();
    }

    public void Delete(Process value)
    {
        throw new NotImplementedException();
    }
}