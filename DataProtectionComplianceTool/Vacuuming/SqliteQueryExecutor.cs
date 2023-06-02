using System.Data;
using Dapper;

namespace GraphManipulation.Vacuuming;

public class SqliteQueryExecutor : IQueryExecutor
{
    private readonly IDbConnection _connection;

    public SqliteQueryExecutor(IDbConnection connection)
    {
        _connection = connection;
    }

    public void Execute(string query)
    {
        _connection.Execute(query);
    }
}