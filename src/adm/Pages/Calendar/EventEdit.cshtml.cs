using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Calendar;
using FamilyHub.Adm.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyHub.Adm.Pages.Calendar;

public class EventEditPageModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    [BindProperty]
    public CalendarEventEditModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> FamilyMemberOptions { get; private set; } = [];

    public IReadOnlyList<SelectListItem> RecurrenceTypeOptions { get; } =
    [
        new SelectListItem("Daglig", "Daily"),
        new SelectListItem("Ugentlig", "Weekly"),
        new SelectListItem("Maanedlig", "Monthly")
    ];

    public IReadOnlyList<DayOption> RecurrenceDayOptions { get; } =
    [
        new(1, "Man"),
        new(2, "Tir"),
        new(3, "Ons"),
        new(4, "Tor"),
        new(5, "Fre"),
        new(6, "Loer"),
        new(0, "Soen")
    ];

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var ev = await _calendarApiClient.GetCalendarEventByIdAsync(id, cancellationToken);
            Input = new CalendarEventEditModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                EventDate = ev.EventDate,
                StartTime = ev.StartTime,
                EndTime = ev.EndTime,
                FamilyMemberId = ev.FamilyMemberId,
                RecurrenceType = ev.RecurrenceType,
                RecurrenceDays = ev.RecurrenceDays?.ToList() ?? []
            };

            await LoadFamilyMembersAsync(cancellationToken);
            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Calendar/Events");
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await LoadFamilyMembersAsync(cancellationToken);
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _calendarApiClient.UpdateCalendarEventAsync(id, ToUpdateRequest(Input), cancellationToken);
            TempData["SuccessMessage"] = "Kalenderbegivenhed opdateret.";
            return RedirectToPage("/Calendar/Events");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }

    private async Task LoadFamilyMembersAsync(CancellationToken cancellationToken)
    {
        var members = await _calendarApiClient.GetFamilyMembersAsync(cancellationToken);
        FamilyMemberOptions = members
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToArray();
    }

    private static UpdateCalendarEventRequest ToUpdateRequest(CalendarEventEditModel input)
    {
        var recurrenceType = string.IsNullOrWhiteSpace(input.RecurrenceType) ? null : input.RecurrenceType;

        return new UpdateCalendarEventRequest
        {
            Title = input.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim(),
            EventDate = input.EventDate,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            FamilyMemberId = input.FamilyMemberId,
            RecurrenceType = recurrenceType,
            RecurrenceDays = recurrenceType is null || input.RecurrenceDays.Count == 0
                ? null
                : input.RecurrenceDays.Distinct().OrderBy(x => x).ToArray()
        };
    }

    public sealed record DayOption(int Value, string Label);
}
