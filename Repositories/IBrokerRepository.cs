using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IBrokerRepository
{
    Task<IEnumerable<BrokerInfo>> ListAllBrokersAsync();
}
