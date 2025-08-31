using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class NotificationType
{ 
    [Key]
    public int NotificationTypeId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Type { get; set; } = string.Empty; // e.g. User Created, Order Updated, etc.

    public bool IsActive { get; set; } = true;
}

public class NotificationDetails
{
    [Key]
    public int NotificationId { get; set; }

    [Required]
    [ForeignKey(nameof(NotificationType))]
    public int NotificationTypeId { get; set; }

    public NotificationType? NotificationType { get; set; }

    [Required]
    public string NotificationContent { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? RedirectURL { get; set; }

    // Foreign key to AspNetUsers
    [Required]
    [ForeignKey("Aspnetusers")]
    public string NotificationGeneratedBy { get; set; } = string.Empty;

    public DateTime NotificationCreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key to AspNetUsers
    [Required]
    [ForeignKey("Aspnetusers")]
    public string NotificationRecepient { get; set; }
}
