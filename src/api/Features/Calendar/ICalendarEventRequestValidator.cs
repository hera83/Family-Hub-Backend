using FamilyHub.Api.Contracts.Calendar;

namespace FamilyHub.Api.Features.Calendar;

public interface ICalendarEventRequestValidator
{
    void ValidateFilter(DateOnly? fromDate, DateOnly? toDate);
    void Validate(CreateCalendarEventRequest request);
    void Validate(UpdateCalendarEventRequest request);
}
