using FamilyHub.Api.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class ItemCategoryConfiguration : IEntityTypeConfiguration<ItemCategory>
{
    public void Configure(EntityTypeBuilder<ItemCategory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.SortOrder);

        builder.HasMany(x => x.Products)
            .WithOne(x => x.ItemCategory)
            .HasForeignKey(x => x.ItemCategoryId)
            // SetNull: produkter mister blot kategorien – slettes ikke
            .OnDelete(DeleteBehavior.SetNull);
    }
}
