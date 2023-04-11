namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public List<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public List<DeletionExecution> Execute();
}