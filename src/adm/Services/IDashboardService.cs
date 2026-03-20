using FamilyHub.Adm.Models.Dashboard;

namespace FamilyHub.Adm.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}
