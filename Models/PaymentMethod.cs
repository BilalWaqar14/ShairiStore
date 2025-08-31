using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class PaymentMethod
{
    [Key]
    [Required]
    public int PaymentMethodId { get; set; } // PK
    public string PaymentMode { get; set; } // e.g. "Bank Transfer", "Credit Card", "Cash", "Online Wallet"
    public bool IsActive { get; set; }
}
