using System.Data;
using GraphManipulation.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphManipulation.DataAccess;

public class GdprMetadataContext : DbContext
{
    private readonly string _connectionString;

    public GdprMetadataContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
    
    public DbSet<PersonalDataColumn> columns { get; set; }
    public DbSet<VacuumingRule> vacuumingRules { get; set; }
    public DbSet<Processing> processes { get; set; }
    public DbSet<Person> people { get; set; }
    public DbSet<Purpose> purposes { get; set; }
    public DbSet<Origin> origins { get; set; }
    
    public IDbConnection Connection => Database.GetDbConnection();

}