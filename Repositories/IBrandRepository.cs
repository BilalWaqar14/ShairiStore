using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IBrandRepository
{
    Task<IEnumerable<Brand>> ListAllBrandsAsync();
}
