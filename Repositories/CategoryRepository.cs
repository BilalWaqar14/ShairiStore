using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderCategory>> ListAllCategoriesAsync()
    {
        return await _context.Categories.OrderByDescending(x => x.CategoryId).ToListAsync();
    }

    public async Task<OrderCategory?> GetByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<OrderCategory> AddAsync(OrderCategory category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<OrderCategory?> UpdateAsync(OrderCategory category)
    {
        var existing = await _context.Categories.FindAsync(category.CategoryId);
        if (existing == null)
            return null;

        existing.CategoryName = category.CategoryName;
        existing.IsActive = category.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing == null)
            return false;

        _context.Categories.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

}