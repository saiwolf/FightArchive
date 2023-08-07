using System.Diagnostics;
using FightArchive.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FightArchive.Data;

public class DataContext : DbContext
{
    /// <summary>
    /// Magic string.
    /// </summary>
    public static readonly string RowVersion = nameof(RowVersion);

    /// <summary>
    /// Magic strings.
    /// </summary>
    public static readonly string FightsDb = nameof(FightsDb).ToLower();


    /// <summary>
    /// Inject options.
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions{ContactContext}"/>
    /// for the context
    /// </param>
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
        Debug.WriteLine($"{ContextId} context created.");
    }

    /// <summary>
    /// List of <see cref="Fight"/>s.
    /// </summary>
    public DbSet<Fight>? Fights { get; set; }

    /// <summary>
    /// List of <see cref="Contender"/>s.
    /// </summary>
    public DbSet<Contender>? Contenders { get; set; }

    public DbSet<Account>? Accounts { get; set; }

    public DbSet<Role>? Roles { get; set; }

    /// <summary>
    /// Define the model.
    /// </summary>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fight>()
            .Property<byte[]>(RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Contender>()
            .Property<byte[]>(RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Account>()
            .Property<byte[]>(RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Role>()
            .Property<byte[]>(RowVersion)
            .IsRowVersion();

        modelBuilder.Entity<Fight>()
            .HasMany(m => m.Contenders)
            .WithMany(w => w.Fights);

        modelBuilder.Entity<Contender>()
            .HasOne(h => h.Account)
            .WithMany(w => w.Characters);

        modelBuilder.Entity<Account>()
            .HasOne(h => h.Role)
            .WithMany(w => w.Accounts);

        modelBuilder.Entity<Account>()
            .HasIndex(i => i.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Dispose pattern.
    /// </summary>
    public override void Dispose()
    {
        Debug.WriteLine($"{ContextId} context disposed.");        
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    /// <summary>
    /// Dispose pattern.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/></returns>
    public override ValueTask DisposeAsync()
    {
        Debug.WriteLine($"{ContextId} context disposed async.");                
        GC.SuppressFinalize(this);
        return base.DisposeAsync();
    }
}
