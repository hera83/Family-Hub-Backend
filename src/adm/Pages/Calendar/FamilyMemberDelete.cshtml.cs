using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.Adm.Pages.Calendar;

public class FamilyMemberDeleteModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    [BindProperty]
    public FamilyMemberListItemViewModel? Item { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var member = await _calendarApiClient.GetFamilyMemberByIdAsync(id, cancellationToken);
            Item = new FamilyMemberListItemViewModel
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

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Item is null || Item.Id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Ugyldigt familiemedlem.";
            return RedirectToPage("/Calendar/FamilyMembers");
        }

        try
        {
            await _calendarApiClient.DeleteFamilyMemberAsync(Item.Id, cancellationToken);
            TempData["SuccessMessage"] = "Familiemedlem slettet.";
        }
        catch (ApiClientException ex)
        {
            TempData["ErrorMessage"] = ex.UserMessage;
        }

        return RedirectToPage("/Calendar/FamilyMembers");
    }
}
