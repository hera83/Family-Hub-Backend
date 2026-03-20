using FamilyHub.Api.Entities.Calendar;
using FamilyHub.Api.Entities.Catalog;
using FamilyHub.Api.Entities.Orders;
using FamilyHub.Api.Entities.Recipes;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Infrastructure.Persistence;

/// <summary>
/// Primær EF Core database-kontekst for hele FamilyHub-løsningen.
/// Registrér nye entiteter som DbSet herinde og konfigurér dem i OnModelCreating.
/// </summary>
public class FamilyHubDbContext(DbContextOptions<FamilyHubDbContext> options) : DbContext(options)
{
    // ─── Calendar ────────────────────────────────────────────────────────────
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

    // ─── Catalog ─────────────────────────────────────────────────────────────
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<Product> Products => Set<Product>();

    // ─── Recipes ─────────────────────────────────────────────────────────────
    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    // ─── Orders ──────────────────────────────────────────────────────────────
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Anvend alle konfigurationer defineret i samme assembly (IEntityTypeConfiguration<T>)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FamilyHubDbContext).Assembly);
    }

    /// <summary>
    /// Automatisk sæt CreatedAtUtc/UpdatedAtUtc ved SaveChanges.
    /// </summary>
    public override int SaveChanges()
    {
        SetAuditDates();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditDates()
    {
        var entries = ChangeTracker.Entries<Entities.BaseEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAtUtc = now;

            // UpdatedAtUtc sættes kun ved opdatering (forbliver null ved første oprettelse)
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAtUtc = now;
        }
    }
}
