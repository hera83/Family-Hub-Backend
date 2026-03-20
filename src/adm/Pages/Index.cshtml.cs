using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyHub.Adm.Models.Dashboard;
using FamilyHub.Adm.Services;

namespace FamilyHub.Adm.Pages;

public class IndexModel(IDashboardService dashboardService) : PageModel
{
    private readonly IDashboardService _dashboardService = dashboardService;

    public DashboardViewModel Dashboard { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Dashboard = await _dashboardService.GetDashboardAsync(cancellationToken);
    }
}
