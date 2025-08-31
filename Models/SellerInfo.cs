using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class SellerInfo
{
    [Key]
    public int SellerId { get; set; }

    [Required]
    public string SellerName { get; set; }

    public bool IsActive { get; set; }
}
