using Microsoft.AspNetCore.Identity;

namespace ShairiStore;

public class ApplicationUser : IdentityUser
{
    // add extra properties if you need
    public string? FullName { get; set; }
}