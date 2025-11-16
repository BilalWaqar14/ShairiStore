using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface ISellerRepository
{
    Task<IEnumerable<SellerInfo>> ListAllSellersAsync();
     Task<SellerInfo?> GetByIdAsync(int id);
    Task<SellerInfo> AddAsync(SellerInfo seller);
    Task<SellerInfo?> UpdateAsync(SellerInfo seller);
    Task<bool> DeleteAsync(int id);
}