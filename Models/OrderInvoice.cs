using Mysqlx.Crud;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShairiStore.Models;

public class OrderInvoice
{
    [Key]
    public int InvoiceId { get; set; } // PK

    [Required]
    [ForeignKey("Orders")]
    public int OrderId { get; set; }   // FK to Orders table

    //[JsonIgnore]  // 🚀 prevents infinite loop
    public Order? Orders { get; set; }

    [Required]
    public double InvoiceAmount { get; set; }

    [Required]
    public double PendingAmount { get; set; }

    public string PaymentScreenshot { get; set; }
    public DateTime InvoiceDate { get; set; }

    [Required]
    public string InvoiceBy { get; set; }

    [Required]
    [ForeignKey("InvoiceStatus")]
    public int InvoiceStatusId { get; set; }  // FK -> InvoiceStatus
    public InvoiceStatus InvoiceStatus { get; set; }

    [Required]
    public string UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(InvoiceBy))]
    public ApplicationUser? User { get; set; }
}

