using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class ExpenseStatus
{
    [Key]
    [Required]
    public int ExpenseStatusId { get; set; } // PK
    public string StatusName { get; set; } // e.g. "Pending", "Paid", "Partially Paid", "Cancelled", "Overdue"
    public bool IsActive { get; set; }
}
