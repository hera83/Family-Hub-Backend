using FamilyHub.Api.Entities.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(x => x.Id);

        // Name er optional – bruges som fritekst fallback når ProductId er sat
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.Quantity).HasPrecision(10, 3);

        builder.HasIndex(x => x.RecipeId);
        builder.HasIndex(x => x.ProductId);

        // SetNull: produktet kan slettes uden at fjerne ingredienslinjen.
        // Ingrediensen bevarer sit Name-felt som visnings-fallback.
        builder.HasOne(x => x.Product)
            .WithMany(x => x.RecipeIngredients)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
