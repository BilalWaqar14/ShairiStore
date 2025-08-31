using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? RoleName { get; set; }
}
