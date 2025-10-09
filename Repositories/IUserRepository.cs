using Microsoft.AspNetCore.Identity;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<UserDTO>> ListAllUsersAsync();
    // ✅ Get user by Id
    Task<ApplicationUser?> GetUserByIdAsync(string userId);

    // ✅ Update user (not password)
    Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user);

    // ✅ Delete user
    Task<ApplicationUser?> DeleteUserAsync(string userId);

    // ✅ Assign role to user
    Task<ApplicationUser?> AssignRoleAsync(string userId, string roleName);
}