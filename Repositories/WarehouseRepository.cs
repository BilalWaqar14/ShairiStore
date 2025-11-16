using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly AppDbContext _context;

    public WarehouseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouses.OrderByDescending(x => x.WarehouseId).ToListAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _context.Warehouses.FindAsync(id);
    }

    public async Task<Warehouse> AddAsync(Warehouse warehouse)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
        return warehouse;
    }

    public async Task<Warehouse?> UpdateAsync(Warehouse warehouse)
    {
        var existing = await _context.Warehouses.FindAsync(warehouse.WarehouseId);
        if (existing == null)
            return null;

        existing.WarehouseName = warehouse.WarehouseName;
        existing.WarehouseAddress = warehouse.WarehouseAddress;
        existing.IsActive = warehouse.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Warehouses.FindAsync(id);
        if (existing == null)
            return false;

        _context.Warehouses.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}