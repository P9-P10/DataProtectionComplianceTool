namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public IEnumerable<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public IEnumerable<DeletionExecution> Execute();
}