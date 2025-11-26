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


    public async Task<IEnumerable<UserDTO>> ListAllUsersAsync()
    {
        var users = await _context.Users
            .Where(x => !string.IsNullOrEmpty(x.Email))
            .ToListAsync();

        IList<UserDTO> result = new List<UserDTO>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var dto = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                RoleName = roles,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = !user.IsDeleted,     // or another field if you use it
                IsDeleted = user.IsDeleted
            };

            result.Add(dto);
        }

        return result.OrderByDescending(x => x.CreatedAt).ToList();
    }

    //// ✅ Get all users
    //public async Task<IEnumerable<UserDTO>> ListAllUsersAsync()
    //{
    //    var users =  await _context.Users.Where(x => !string.IsNullOrEmpty(x.Email)).ToListAsync();
    //    IList<UserDTO> result = new List<UserDTO>();
    //    foreach (var user in users)
    //    {
    //        var roles = await _userManager.GetRolesAsync(user);
    //        var res = new UserDTO();
    //        res.Email = user.Email;
    //        res.FullName = user.FullName;
    //        res.PhoneNumber = user.PhoneNumber;
    //        res.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
    //        res.EmailConfirmed = user.EmailConfirmed;
    //        res.RoleName = roles;
    //        res.Id = user.Id;
    //        result.Add(res);
    //    }
    //    return result.OrderByDescending(x=> x.createdAt).ToList();
    //}

    // ✅ Get user by Id
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        var response = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return response;
    }

    // ✅ Update user (not password)
    //public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user)
    //{
    //    var pass = user.RawPassword;

    //    //var interimUser = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

    //    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
    //    var result = await _userManager.ResetPasswordAsync(user, resetToken, user.RawPassword);



    //    var existingUser = await _userManager.FindByIdAsync(user.Id);

    //    if (existingUser == null)
    //        return null;

    //    existingUser.FullName = user.FullName;
    //    existingUser.Email = user.Email;
    //    existingUser.UserName = user.Email;
    //    existingUser.DisplayPicture = user.DisplayPicture;
    //    existingUser.PhoneNumber = user.PhoneNumber;
    //    existingUser.RawPassword = pass;
        
    //    var resultUser = await _userManager.UpdateAsync(existingUser);

    //    return resultUser.Succeeded ? existingUser : null;
    //}


    public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user)
    {
        var existingUser = await _userManager.FindByIdAsync(user.Id);

        if (existingUser == null)
            return null;

        // 1️⃣ Reset password first (on the tracked existingUser)
        if (!string.IsNullOrWhiteSpace(user.RawPassword))
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var resetResult = await _userManager.ResetPasswordAsync(existingUser, resetToken, user.RawPassword);

            if (!resetResult.Succeeded)
                return null;
        }

        // 2️⃣ Update identity fields
        existingUser.FullName = user.FullName;
        existingUser.Email = user.Email;
        existingUser.UserName = user.Email;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.DisplayPicture = user.DisplayPicture;

        // 3️⃣ Save identity updates
        var updateResult = await _userManager.UpdateAsync(existingUser);

        if (!updateResult.Succeeded)
            return null;

        // 4️⃣ Now store RawPassword (AFTER identity update)
        // Using EF directly, NOT UserManager, so it doesn't affect password hashing
        existingUser.RawPassword = user.RawPassword;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        return existingUser;
    }


    // ✅ Delete user
    public async Task<ApplicationUser?> DeleteUserAsync(string userId)
    {
        //var user = await _userManager.FindByIdAsync(userId);
        //if (user == null)
        //    return null;

        //var result = await _userManager.DeleteAsync(user);

        //return result.Succeeded ? user : null;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

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