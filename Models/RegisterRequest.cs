using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(10)]
    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? RoleName { get; set; }
    public int RoleId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayPicture { get; set; }
}


public class LoginResponseModel
{
    public string Id { get; set; }
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? roles { get; set; }
    public int RoleId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? DisplayPicture { get; set; }
    public string token { get; set; }
}
