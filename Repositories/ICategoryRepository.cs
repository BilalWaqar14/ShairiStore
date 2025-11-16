using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<OrderCategory>> ListAllCategoriesAsync();
    Task<OrderCategory?> GetByIdAsync(int id);
    Task<OrderCategory> AddAsync(OrderCategory category);
    Task<OrderCategory?> UpdateAsync(OrderCategory category);
    Task<bool> DeleteAsync(int id);
}