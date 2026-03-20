using FamilyHub.Api.Entities.Calendar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Api.Infrastructure.Persistence.Configurations;

public class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        // RecurrenceType: begrænset tekst (fx "Weekly")
        builder.Property(x => x.RecurrenceType)
            .HasMaxLength(20);

        // RecurrenceDaysJson: lille JSON-array, fx "[1,3,5]"
        builder.Property(x => x.RecurrenceDaysJson)
            .HasMaxLength(200);

        // Hurtig datofiltrering i kalendervisning
        builder.HasIndex(x => x.EventDate);

        // Filtrering pr. familiemedlem
        builder.HasIndex(x => x.FamilyMemberId);
    }
}
