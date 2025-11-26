using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IDashboardRepository
{
    public Task<DashboardResponse> GetDashboardDataAsync(DashboardRequest request);
}
