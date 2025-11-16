using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // ✅ Get all users
    public async Task<IEnumerable<UserDTO>> ListAllUsersAsync()
    {
        var users =  await _context.Users.Where(x => !string.IsNullOrEmpty(x.Email)).ToListAsync();
        IList<UserDTO> result = new List<UserDTO>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var res = new UserDTO();
            res.Email = user.Email;
            res.FullName = user.FullName;
            res.PhoneNumber = user.PhoneNumber;
            res.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            res.EmailConfirmed = user.EmailConfirmed;
            res.RoleName = roles;
            res.Id = user.Id;
            result.Add(res);
        }
        return result.OrderByDescending(x=> x.createdAt).ToList();
    }

    // ✅ Get user by Id
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    // ✅ Update user (not password)
    public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user)
    {
        var existingUser = await _userManager.FindByIdAsync(user.Id);
        if (existingUser == null)
            return null;

        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
        existingUser.UserName = user.Email;

        var result = await _userManager.UpdateAsync(existingUser);

        return result.Succeeded ? existingUser : null;
    }

    // ✅ Delete user
    public async Task<ApplicationUser?> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded ? user : null;
    }

    // ✅ Assign role to user
    public async Task<ApplicationUser?> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded) return null;
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);

        return result.Succeeded ? user : null;
    }
}