namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public List<string> GenerateSelectStatementForDataToDelete(string predefinedExpirationDate="");
    
}