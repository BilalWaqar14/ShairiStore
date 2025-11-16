using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;
    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Brand>> ListAllBrandsAsync()
    {
        return await _context.Brands.OrderByDescending(x=>x.BrandId).ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<Brand> AddAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand?> UpdateAsync(Brand brand)
    {
        var existing = await _context.Brands.FindAsync(brand.BrandId);
        if (existing == null)
            return null;

        existing.BrandName = brand.BrandName;
        existing.IsActive = brand.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Brands.FindAsync(id);
        if (existing == null)
            return false;

        _context.Brands.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}