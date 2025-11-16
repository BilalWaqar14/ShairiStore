using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IBrokerRepository
{
    Task<IEnumerable<BrokerInfo>> ListAllBrokersAsync();
    Task<BrokerInfo?> GetByIdAsync(int id);
    Task<BrokerInfo> AddAsync(BrokerInfo broker);
    Task<BrokerInfo?> UpdateAsync(BrokerInfo broker);
    Task<bool> DeleteAsync(int id);
}