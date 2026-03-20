using FamilyHub.Api.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Notes).HasMaxLength(4000);
        builder.Property(x => x.TotalPrice).HasPrecision(10, 2);

        // PDF gemmes som rå tekst (base64/data-URL) – ingen grænse på SQLite TEXT
        builder.Property(x => x.PdfData).HasColumnType("TEXT");

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
