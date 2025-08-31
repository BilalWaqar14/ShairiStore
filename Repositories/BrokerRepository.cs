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
        return await _context.Brokers.ToListAsync();
    }
}
