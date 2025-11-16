using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class CreditPayments
{
    [Key]
    public int PaymentId { get; set; } // PK

    [Required]
    [ForeignKey("Expense")]
    public int ExpenseId { get; set; }   // FK to Orders table

    //[JsonIgnore]  // 🚀 prevents infinite loop
    public Expense? Expense { get; set; }

    [Required]
    public double AmountPaid { get; set; }

    [Required]
    public double RemainingAmount { get; set; }

    public string PaymentMethod { get; set; }   // FK -> PaymentMethod

    public string CreditScreenshot { get; set; }
    public DateTime PaymentDate { get; set; }

    [Required]
    public string PaidBy { get; set; }

    [Required]
    [ForeignKey("InvoiceStatus")]
    public int InvoiceStatusId { get; set; }  // FK -> InvoiceStatus
    public InvoiceStatus? InvoiceStatus { get; set; }

    [Required]
    public string UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(PaidBy))]
    public ApplicationUser? User { get; set; }
}
