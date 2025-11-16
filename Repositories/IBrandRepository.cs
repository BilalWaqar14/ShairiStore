using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> ListAllBrandsAsync();
    Task<Brand?> GetByIdAsync(int id);
    Task<Brand> AddAsync(Brand brand);
    Task<Brand?> UpdateAsync(Brand brand);
    Task<bool> DeleteAsync(int id);
}
