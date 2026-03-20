using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Calendar;

public class EventDeleteModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    [BindProperty]
    public CalendarEventListItemViewModel? Item { get; set; }

    public string TimeRange
    {
        get
        {
            if (Item is null)
            {
                return "-";
            }

            var start = Item.StartTime?.ToString("HH:mm") ?? "-";
            var end = Item.EndTime?.ToString("HH:mm") ?? "-";
            return $"{start} - {end}";
        }
    }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ev = await _calendarApiClient.GetCalendarEventByIdAsync(id, cancellationToken);
            Item = new CalendarEventListItemViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                EventDate = ev.EventDate,
                StartTime = ev.StartTime,
                EndTime = ev.EndTime,
                FamilyMemberName = ev.FamilyMemberName,
                FamilyMemberColor = ev.FamilyMemberColor,
                RecurrenceType = ev.RecurrenceType,
                RecurrenceDaysDisplay = ev.RecurrenceDays is null ? null : string.Join(", ", ev.RecurrenceDays)
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Calendar/Events");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldig kalenderbegivenhed.";
            return RedirectToPage("/Calendar/Events");
        }

        try
        {
            await _calendarApiClient.DeleteCalendarEventAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Kalenderbegivenhed slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Calendar/Events");
    }
}
