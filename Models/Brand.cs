using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class Brand
{
    [Key]
    public int BrandId { get; set; }

    [Required]
    public string BrandName { get; set; }

    public bool IsActive { get; set; }
}
