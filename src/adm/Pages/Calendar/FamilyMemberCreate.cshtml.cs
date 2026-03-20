using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Api.Calendar;
using FamilyHub.Adm.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Calendar;

public class FamilyMemberCreateModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    [BindProperty]
    public FamilyMemberEditModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _calendarApiClient.CreateFamilyMemberAsync(new CreateFamilyMemberRequest
            {
                Name = Input.Name.Trim(),
                Color = Input.Color.Trim()
            }, cancellationToken);

            TempData["SuccessMessage"] = "Familiemedlem oprettet.";
            return RedirectToPage("/Calendar/FamilyMembers");
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError(string.Empty, ex.UserMessage);
            return Page();
        }
    }
}
