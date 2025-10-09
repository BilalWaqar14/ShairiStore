using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IInventoryRepository
{
    Task<PagedResult<Inventory>> GetInventoryAsync(int pageNumber, int pageSize);
    Task<Inventory?> UpdateInventoryAsync(int orderId, int subCategoryId, ApplicationUser user);
    Task<double> GetAvailableQuantitybyCategoryAsync(int subCategoryId);
}
