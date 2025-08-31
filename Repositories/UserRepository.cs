using Microsoft.EntityFrameworkCore;

namespace ShairiStore.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationUser>> ListAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}