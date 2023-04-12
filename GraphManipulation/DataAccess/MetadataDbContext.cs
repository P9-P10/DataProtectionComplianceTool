using System.Data;
using GraphManipulation.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace GraphManipulation.DataAccess;

public class MetadataDbContext : DbContext
{
    private readonly string _connectionString;

    public MetadataDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ColumnMetadata>()
            .HasMany(e => e.Metadata)
            .WithOne(e => e.Column)
            .HasForeignKey(e => e.TargetColumn)
            .HasPrincipalKey(e => e.Id);
        
        modelBuilder.Entity<GdprMetadataEntity>()
            .HasMany(e => e.Conditions)
            .WithOne(e => e.MetadataEntity)
            .HasForeignKey(e => e.MetadataId)
            .HasPrincipalKey(e => e.Id);
        
        modelBuilder.Entity<GdprMetadataEntity>()
            .HasMany(e => e.Processes)
            .WithOne(e => e.MetadataEntity)
            .HasForeignKey(e => e.MetadataId)
            .HasPrincipalKey(e => e.Id);
    }
    
    public DbSet<ColumnMetadata> columns { get; set; }
    public DbSet<GdprMetadataEntity> metadata { get; set; }
    public DbSet<ConditionMetadata> conditions { get; set; }
    public DbSet<ProcessingMetadata> processes { get; set; }
    public IDbConnection Connection => Database.GetDbConnection();

}