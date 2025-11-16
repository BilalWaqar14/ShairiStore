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
        return await _context.Sellers.OrderByDescending(x => x.SellerId).ToListAsync();
    }

    public async Task<SellerInfo?> GetByIdAsync(int id)
    {
        return await _context.Sellers.FindAsync(id);
    }

    public async Task<SellerInfo> AddAsync(SellerInfo seller)
    {
        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync();
        return seller;
    }

    public async Task<SellerInfo?> UpdateAsync(SellerInfo seller)
    {
        var existing = await _context.Sellers.FindAsync(seller.SellerId);
        if (existing == null)
            return null;

        existing.SellerName = seller.SellerName;
        existing.IsActive = seller.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Sellers.FindAsync(id);
        if (existing == null)
            return false;

        _context.Sellers.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
