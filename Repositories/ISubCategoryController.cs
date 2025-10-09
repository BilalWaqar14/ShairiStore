using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface ISubCategoryRepository
{
    Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesAsync();
    Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesByCategoryIdAsync(int categoryId);
}