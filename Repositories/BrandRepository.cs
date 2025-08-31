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
        return await _context.Brands.ToListAsync();
    }
}