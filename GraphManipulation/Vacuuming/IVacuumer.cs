namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public List<string> GenerateUpdateStatement(string predefinedExpirationDate = "");

    public List<DeletionExecution> Execute();
}