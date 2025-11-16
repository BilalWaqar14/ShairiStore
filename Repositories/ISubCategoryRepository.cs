using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface ISubCategoryRepository
{
    Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesAsync();
    Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesByCategoryIdAsync(int categoryId);
    Task<OrderSubCategory> GetByIdAsync(int id);
    Task<OrderSubCategory> AddAsync(OrderSubCategory subCategory);
    Task<OrderSubCategory> UpdateAsync(OrderSubCategory subCategory);
    Task<bool> DeleteAsync(int id);
}