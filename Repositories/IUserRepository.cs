namespace ShairiStore.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> ListAllUsersAsync();
}