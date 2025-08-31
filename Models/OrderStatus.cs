using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class OrderStatus
{
    [Key]
    public int StatusId { get; set; }
    public string Status { get; set; }
    public bool IsActive { get; set; }
}
