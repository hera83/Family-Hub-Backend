using FamilyHub.Api.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.HasKey(x => x.Id);

        // Snapshot-felter – ingen FK til Product; historik er uafhængig af kataloget
        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CategoryName).HasMaxLength(100);
        builder.Property(x => x.Unit).HasMaxLength(50);
        builder.Property(x => x.SizeLabel).HasMaxLength(100);
        builder.Property(x => x.Quantity).HasPrecision(10, 3);
        builder.Property(x => x.Price).HasPrecision(10, 2);

        builder.HasIndex(x => x.OrderId);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
