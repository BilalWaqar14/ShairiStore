using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class OrderCategory
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public string CategoryName { get; set; }

    public bool IsActive { get; set; }
}
