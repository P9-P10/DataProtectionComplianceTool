namespace GraphManipulation.Utility;

public static class CreateStatementManipulator
{
    public static string ReplaceCreateTableWithCreateTableIfNotExists(string query)
    {
        return query.Replace("CREATE TABLE", "CREATE TABLE IF NOT EXISTS");
    }

    public static string ReplaceCreateIndexWithCreateIndexIfNotExists(string query)
    {
        return query.Replace("CREATE INDEX", "CREATE INDEX IF NOT EXISTS");
    }

    public static string UpdateCreationScript(string query)
    {
        string output = ReplaceCreateTableWithCreateTableIfNotExists(query);
        return ReplaceCreateIndexWithCreateIndexIfNotExists(output);
    }
}