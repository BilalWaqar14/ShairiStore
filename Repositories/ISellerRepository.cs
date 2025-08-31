using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface ISellerRepository
{
    Task<IEnumerable<SellerInfo>> ListAllSellersAsync();
}
