using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class OrderType
{
    [Key]
    public int OrderTypeId { get; set; }

    [Required]
    public string TypeName { get; set; }

    public bool IsActive { get; set; }
}