using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShairiStore.Models;

public class Order
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

    [Required]
    [ForeignKey("BrokerInfo")]
    public int BrokerId { get; set; }
    public BrokerInfo? Broker { get; set; }

    [Required]
    public double BrokerAmount { get; set; }

    public string? OrderNotes { get; set; }
    public string? OrderScreenShot { get; set; }

    [Required]
    public double CalculatedAmount { get; set; }

    [Required]
    public double OtherExpenses { get; set; }

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


    [Required]
    public double TotalRequiredQuantity { get; set; }

    [Required]
    public double TotalRequiredKgs { get; set; }

    public double? TotalReceivedQuantity { get; set; }
    public double? TotalReceivedKgs { get; set; }

    public double? TotalPendingBalance { get; set; }
    public double? TotalPendingQuantity { get; set; }
    public double? TotalPendingKgs { get; set; }

    public string OrderName { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; }

    public OrderInvoice? OrderInvoice { get; set; }

    public ICollection<OrderPayment?> OrderPayments { get; set; }
}

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }

    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [JsonIgnore]  // 🚀 prevents infinite loop
    public Order? Order { get; set; }

    //[Required]
    //[ForeignKey("OrderCategory")]
    //public int CategoryId { get; set; }
    //public OrderCategory Category { get; set; }

    [Required]
    [ForeignKey("OrderSubCategory")]
    public int SubCategoryId { get; set; }
    public OrderSubCategory? OrderSubCategory { get; set; }

    //[Required]
    //[ForeignKey("Brand")]
    //public int BrandId { get; set; }
    //public Brand Brand { get; set; }

    [Required]
    public double OneMonRate { get; set; }

    [Required]
    public double RequiredQuantity { get; set; }

    [Required]
    public double RequiredKgs { get; set; }

    [Required]
    public double CalculatedAmount { get; set; }

    [Required]
    public double OtherExpenses { get; set; }

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

    public double? ReceivedQuantity { get; set; }
    public double? ReceivedKgs { get; set; }
    public double? PendingBalance { get; set; }
    public double? PendingQuantity { get; set; }
    public double? PendingKgs { get; set; }

    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OrderPayment
{
    [Key]
    public int PaymentId { get; set; } // PK

    [Required]
    [ForeignKey("Order")]
    public int OrderId { get; set; }   // FK to Orders table

    //[JsonIgnore]  // 🚀 prevents infinite loop
    public Order? Order { get; set; }

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
