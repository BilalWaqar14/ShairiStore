using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<OrderCategory>> ListAllCategoriesAsync();
}

