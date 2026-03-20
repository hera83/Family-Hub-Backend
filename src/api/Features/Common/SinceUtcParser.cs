using System.Globalization;

namespace FamilyHub.Api.Features.Common;

internal static class SinceUtcParser
{
    public static DateTime ValidateAndParse(string? sinceUtc)
    {
        if (string.IsNullOrWhiteSpace(sinceUtc))
            throw new ArgumentException("sinceUtc er påkrævet og skal være et gyldigt ISO 8601 UTC-tidspunkt.");

        var parsed = DateTimeOffset.TryParse(
            sinceUtc,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var dateTimeOffset);

        if (!parsed || dateTimeOffset.Offset != TimeSpan.Zero)
            throw new ArgumentException("sinceUtc skal være et gyldigt ISO 8601 UTC-tidspunkt, fx 2026-03-19T08:30:00Z.");

        return dateTimeOffset.UtcDateTime;
    }
}
