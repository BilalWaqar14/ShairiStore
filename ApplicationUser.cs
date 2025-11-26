using Microsoft.AspNetCore.Identity;

namespace ShairiStore;

public class ApplicationUser : IdentityUser
{
    // add extra properties if you need
    public string? FullName { get; set; }
    // ⭐ Soft Delete Fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Optional: for auditing
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    //public string PhoneNumbner { get; set; }
    public string DisplayPicture { get; set; }
    public string RoleName { get; set; }

    public string? RawPassword { get; set; }
}

public class UserDTO
{
    public IList<string?> RoleName { get; set; }
    public string Id { get; set; }          // IdentityUser Id (string by default)
    public string Email { get; set; }       // Email of the user
    public string UserName { get; set; }    // Username (optional)
    public string FullName { get; set; }    // Custom field if you have one
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }      // Optional status field

    public bool PhoneNumberConfirmed { get; set; }
    public bool EmailConfirmed { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public string RawPassword { get; set; }

}

