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
        return await _context.SubCategories.OrderByDescending(x=>x.SubCategoryId).ToListAsync();
    }

    public async Task<IEnumerable<OrderSubCategory>> ListAllSubCategoriesByCategoryIdAsync(int categoryId)
    {
        return await _context.SubCategories.Where(x=> x.CategoryId == categoryId).Include(x=> x.Brand).Include(x=> x.Category).ToListAsync();
    }

    public async Task<OrderSubCategory?> GetByIdAsync(int id)
    {
        return await _context.SubCategories
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .FirstOrDefaultAsync(x => x.SubCategoryId == id);
    }

    public async Task<OrderSubCategory> AddAsync(OrderSubCategory subCategory)
    {
        subCategory.BrandId = subCategory.Brand.BrandId;
        subCategory.CategoryId = subCategory.Category.CategoryId;
        subCategory.Brand = null;
        subCategory.Category = null;
        _context.SubCategories.Add(subCategory);
        await _context.SaveChangesAsync();
        return subCategory;
    }

    public async Task<OrderSubCategory> UpdateAsync(OrderSubCategory subCategory)
    {
        // Load the existing tracked entity
        var existing = await _context.SubCategories
            .FirstOrDefaultAsync(x => x.SubCategoryId == subCategory.SubCategoryId);

        if (existing == null)
            throw new Exception("Subcategory not found");

        // Update properties manually
        existing.SubCategoryName = subCategory.SubCategoryName;
        existing.IsActive = subCategory.IsActive;
        existing.CategoryId = subCategory.Category.CategoryId;
        existing.BrandId = subCategory.Brand.BrandId;
        existing.OneMonRate = subCategory.OneMonRate;

        await _context.SaveChangesAsync();

        return existing;

    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.SubCategories.FindAsync(id);
        if (entity == null)
            return false;

        _context.SubCategories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}