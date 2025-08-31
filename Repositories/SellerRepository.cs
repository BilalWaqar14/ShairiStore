using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class SellerRepository : ISellerRepository
{
    private readonly AppDbContext _context;
    public SellerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SellerInfo>> ListAllSellersAsync()
    {
        return await _context.Sellers.ToListAsync();
    }
}
