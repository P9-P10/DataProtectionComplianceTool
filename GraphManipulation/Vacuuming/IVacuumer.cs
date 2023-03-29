namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public List<string> GenerateSqlQueryForDeletion(string predefinedExpirationDate);
    
}