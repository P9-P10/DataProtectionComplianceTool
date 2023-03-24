namespace GraphManipulation.Vacuuming;

public interface IVacuumer
{
    public bool DeleteExpiredTuples();

    public string GenerateSqlQueryForDeletion(string predefinedExpirationDate);
}