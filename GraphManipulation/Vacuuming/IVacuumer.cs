namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public string GenerateSqlQueryForDeletion(string predefinedExpirationDate);
}