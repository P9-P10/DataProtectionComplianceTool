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

    public DbSet<PersonalDataColumn> columns { get; set; }
    public DbSet<VacuumingPolicy> vacuumingPolicies { get; set; }
    public DbSet<Processing> processes { get; set; }
    public DbSet<Individual> people { get; set; }
    public DbSet<LegalBasis> legalBases { get; set; }

    public DbSet<ConfigKeyValue> individualsSourceStores { get; set; }
    public DbSet<Purpose> purposes { get; set; }
    public DbSet<Origin> origins { get; set; }
    public DbSet<PersonalDataOrigin> personalDatas { get; set; }
    public DbSet<StoragePolicy> storagePolicies { get; set; }

    public IDbConnection Connection => Database.GetDbConnection();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonalDataColumn>().OwnsOne(p => p.Key);
        modelBuilder.Entity<Purpose>().HasMany<StoragePolicy>(p => p.StoragePolicies);
    }
}