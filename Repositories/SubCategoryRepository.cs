using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly AppDbContext _context;
    public SubCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesAsync()
    {
        return await _context.SubCategories.ToListAsync();
    }

    public async Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesByCategoryIdAsync(int categoryId)
    {
        return await _context.SubCategories.Where(x=> x.CategoryId == categoryId).Include(x=> x.Brand).Include(x=> x.Category).ToListAsync();
    }
}
