using Microsoft.AspNetCore.Identity;

namespace ShairiStore;

public class ApplicationUser : IdentityUser
{
    // add extra properties if you need
    public string? FullName { get; set; }    
    //public string PhoneNumbner { get; set; }
    //public string DisplayPic { get; set; }
}

public class UserDTO : ApplicationUser
{
    public IList<string?> RoleName { get; set; }
    public string Id { get; set; }          // IdentityUser Id (string by default)
    public string Email { get; set; }       // Email of the user
    public string UserName { get; set; }    // Username (optional)
    public string FullName { get; set; }    // Custom field if you have one
    public string PhoneNumber { get; set; }
    public string createdAt { get; set; }
    public string updatedAt { get; set; }
    public bool IsActive { get; set; }      // Optional status field
}

