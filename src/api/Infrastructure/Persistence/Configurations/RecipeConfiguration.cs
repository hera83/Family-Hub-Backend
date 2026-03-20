using FamilyHub.Api.Entities.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ImageUrl).HasMaxLength(1000);
        builder.Property(x => x.Description).HasMaxLength(4000);
        builder.Property(x => x.Instructions).HasMaxLength(20000);

        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.RecipeCategoryId);
        builder.HasIndex(x => x.IsFavorite);

        builder.HasMany(x => x.Ingredients)
            .WithOne(x => x.Recipe)
            .HasForeignKey(x => x.RecipeId)
            // Cascade: en ingredienslinje er meningsløs uden sin opskrift
            .OnDelete(DeleteBehavior.Cascade);
    }
}
