using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class BrokerRepository : IBrokerRepository
{
    private readonly AppDbContext _context;
    public BrokerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BrokerInfo>> ListAllBrokersAsync()
    {
        return await _context.Brokers.OrderByDescending(x => x.BrokerId).ToListAsync();
    }

    public async Task<BrokerInfo?> GetByIdAsync(int id)
    {
        return await _context.Brokers.FindAsync(id);
    }

    public async Task<BrokerInfo> AddAsync(BrokerInfo broker)
    {
        _context.Brokers.Add(broker);
        await _context.SaveChangesAsync();
        return broker;
    }

    public async Task<BrokerInfo?> UpdateAsync(BrokerInfo broker)
    {
        var existing = await _context.Brokers.FindAsync(broker.BrokerId);
        if (existing == null)
            return null;

        existing.BrokerName = broker.BrokerName;
        existing.BrokerCommission = broker.BrokerCommission;
        existing.IsActive = broker.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Brokers.FindAsync(id);
        if (existing == null)
            return false;

        _context.Brokers.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
