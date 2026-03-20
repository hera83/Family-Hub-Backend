using Microsoft.Extensions.DependencyInjection;

namespace FamilyHub.Api.Features.Calendar;

/// <summary>
/// Registrerer alle Calendar-feature services i DI-containeren.
/// Kaldes fra Program.cs.
/// </summary>
public static class CalendarServiceExtensions
{
    public static IServiceCollection AddCalendarServices(this IServiceCollection services)
    {
        services.AddScoped<IFamilyMemberRequestValidator, FamilyMemberRequestValidator>();
        services.AddScoped<ICalendarEventRequestValidator, CalendarEventRequestValidator>();
        services.AddScoped<ICalendarSyncRequestValidator, CalendarSyncRequestValidator>();
        services.AddScoped<IFamilyMemberService, FamilyMemberService>();
        services.AddScoped<ICalendarEventService, CalendarEventService>();
        services.AddScoped<ICalendarSyncService, CalendarSyncService>();
        return services;
    }
}
