using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Calendar;
using FamilyHub.Adm.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Calendar;

public class FamilyMemberEditPageModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    [BindProperty]
    public FamilyHub.Adm.Models.Calendar.FamilyMemberEditModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var member = await _calendarApiClient.GetFamilyMemberByIdAsync(id, cancellationToken);
            Input = new FamilyHub.Adm.Models.Calendar.FamilyMemberEditModel
            {
                Id = member.Id,
                Name = member.Name,
                Color = member.Color
            };

            return Page();
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
            return RedirectToPage("/Calendar/FamilyMembers");
        }
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _calendarApiClient.UpdateFamilyMemberAsync(id, new UpdateFamilyMemberRequest
            {
                Name = Input.Name.Trim(),
                Color = Input.Color.Trim()
            }, cancellationToken);

            TempData["SuccessMessage"] = "Familiemedlem opdateret.";
            return RedirectToPage("/Calendar/FamilyMembers");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }
}
