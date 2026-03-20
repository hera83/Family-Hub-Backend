using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Infrastructure.Clients.Calendar;
using FamilyHub.Adm.Infrastructure.Clients.Common;
using FamilyHub.Adm.Models.Calendar;

namespace FamilyHub.Adm.Pages.Calendar;

public class FamilyMembersModel(ICalendarApiClient calendarApiClient) : PageModel
{
    private readonly ICalendarApiClient _calendarApiClient = calendarApiClient;

    public IReadOnlyList<FamilyMemberListItemViewModel> Items { get; private set; } = [];

    public string? LoadErrorMessage { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var members = await _calendarApiClient.GetFamilyMembersAsync(cancellationToken);
            Items = members
                .OrderBy(x => x.Name)
                .Select(x => new FamilyMemberListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Color = x.Color
                })
                .ToArray();
        }
        catch (ApiClientException ex)
        {
            LoadErrorMessage = ex.UserMessage;
        }
    }
}
