using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShairiStore.Models;

public class OutgoingOrder
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    [ForeignKey("OrderType")]
    public int OrderTypeId { get; set; }
    public OrderType? OrderType { get; set; }

    [Required]
    [ForeignKey("SellerInfo")]
    public int SellerId { get; set; }
    public SellerInfo? Seller { get; set; }

    public string? OrderNotes { get; set; }
    public string? OrderScreenShot { get; set; }

    [Required]
    public double TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }

    [Required]
    public string OrderBy { get; set; }   // FK to AspNetUsers.Id (string PK)

    [ForeignKey(nameof(OrderBy))]
    public ApplicationUser? User { get; set; }

    [Required]
    [ForeignKey("OrderStatus")]
    public int OrderStatusId { get; set; }
    public OrderStatus? OrderStatus { get; set; }

    [Required]
    [ForeignKey("Warehouse")]
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public double? TotalOrderQuantity { get; set; }
    public double? TotalOrderKgs { get; set; }

    public string OrderName { get; set; }

    public string CustomerName { get; set; }

    public ICollection<OutgoingOrderDetails> OutgoingOrderDetails { get; set; }

    public OutgoingOrderInvoice? OrderInvoice { get; set; }

    public OutgoingOrderPayment? OutgoingOrderPayments { get; set; }
}

public class OutgoingOrderDetails
{
    [Key]
    public int OutgoingOrderDetailId { get; set; }

    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [JsonIgnore]  // 🚀 prevents infinite loop
    public OutgoingOrder? Order { get; set; }

    [Required]
    [ForeignKey("OrderSubCategory")]
    public int SubCategoryId { get; set; }
    public OrderSubCategory? OrderSubCategory { get; set; }

    [Required]
    public double OrderQuantity { get; set; }

    [Required]
    public double OrderKgs { get; set; }

    [Required]
    public double TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }

    [Required]
    public string OrderBy { get; set; }   // AspNetUsers FK

    [ForeignKey(nameof(OrderBy))]
    public ApplicationUser? User { get; set; }

    [Required]
    [ForeignKey("OrderStatus")]
    public int OrderStatusId { get; set; }

    public OrderStatus? OrderStatus { get; set; }

    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OutgoingOrderPayment
{
    [Key]
    public int PaymentId { get; set; } // PK

    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }   // FK to Orders table

    //[JsonIgnore]  // 🚀 prevents infinite loop
    public OutgoingOrder? Order { get; set; }

    [Required]
    public double AmountPaid { get; set; }

    [Required]
    public double RemainingAmount { get; set; }

    [Required]
    [ForeignKey("PaymentMethod")]
    public int PaymentMethodId { get; set; }   // FK -> PaymentMethod
    public PaymentMethod? PaymentMethod { get; set; }

    public string PaymentScreenshot { get; set; }
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


public class OutgoingOrderInvoice
{
    [Key]
    public int InvoiceId { get; set; } // PK

    [Required]
    [ForeignKey("Orders")]
    public int OrderId { get; set; }   // FK to Orders table

    //[JsonIgnore]  // 🚀 prevents infinite loop
    public OutgoingOrder? Orders { get; set; }

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
    public InvoiceStatus? InvoiceStatus { get; set; }

    [Required]
    public string UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(InvoiceBy))]
    public ApplicationUser? User { get; set; }
}
