using FamilyHub.Api.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.ImageUrl).HasMaxLength(1000);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.SizeLabel).HasMaxLength(100);

        // SQLite har ikke native DECIMAL; EF Core SQLite lagrer decimal som TEXT for at bevare præcision
        builder.Property(x => x.Price).HasPrecision(10, 2);
        builder.Property(x => x.CaloriesPer100g).HasPrecision(8, 2);
        builder.Property(x => x.FatPer100g).HasPrecision(7, 3);
        builder.Property(x => x.CarbsPer100g).HasPrecision(7, 3);
        builder.Property(x => x.ProteinPer100g).HasPrecision(7, 3);
        builder.Property(x => x.FiberPer100g).HasPrecision(7, 3);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.ItemCategoryId);
        builder.HasIndex(x => x.IsFavorite);
        builder.HasIndex(x => x.IsStaple);
    }
}
