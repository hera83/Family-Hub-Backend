using FamilyHub.Api.Entities.Calendar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
{
    public void Configure(EntityTypeBuilder<FamilyMember> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("#000000");

        // Indeks for hurtig søgning på navn
        builder.HasIndex(x => x.Name);

        builder.HasMany(x => x.CalendarEvents)
            .WithOne(x => x.FamilyMember)
            .HasForeignKey(x => x.FamilyMemberId)
            // SetNull: begivenheder bevares med historik ved sletning af familiemedlem
            .OnDelete(DeleteBehavior.SetNull);
    }
}
