using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShairiStore.Models;

public class OrderInvoice
{
    [Key]
    [Required]
    public int InvoiceId { get; set; } // PK

    [Required]
    [ForeignKey("Orders")]
    public int OrderId { get; set; }   // FK to Orders table

    [Required]
    public double InvoiceAmount { get; set; }

    [Required]
    public double OrderAmount { get; set; }

    [Required]
    public double PendingAmount { get; set; }

    [Required]
    [ForeignKey("PaymentMethod")]
    public int PaymentMethodId { get; set; }   // FK -> PaymentMethod
    public PaymentMethod PaymentMethod { get; set; }

    public string PaymentScreenshot { get; set; }
    public DateTime InvoiceDate { get; set; }

    [Required]
    [ForeignKey("AspNetUsers")]
    public string InvoiceBy { get; set; }

    [Required]
    [ForeignKey("InvoiceStatus")]
    public int InvoiceStatusId { get; set; }  // FK -> InvoiceStatus
    public InvoiceStatus InvoiceStatus { get; set; }

    [Required]
    [ForeignKey("AspNetUsers")]
    public string UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

