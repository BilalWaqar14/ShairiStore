using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class InvoiceStatus
{
    [Key]
    [Required]
    public int InvoiceStatusId { get; set; } // PK
    public string StatusName { get; set; } // e.g. "Pending", "Paid", "Partially Paid", "Cancelled", "Overdue"
    public bool IsActive { get; set; }
}
