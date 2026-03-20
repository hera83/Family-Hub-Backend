using FamilyHub.Api.Entities.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class RecipeCategoryConfiguration : IEntityTypeConfiguration<RecipeCategory>
{
    public void Configure(EntityTypeBuilder<RecipeCategory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.SortOrder);

        builder.HasMany(x => x.Recipes)
            .WithOne(x => x.RecipeCategory)
            .HasForeignKey(x => x.RecipeCategoryId)
            // SetNull: opskrifter mister blot kategorien – slettes ikke
            .OnDelete(DeleteBehavior.SetNull);
    }
}
