using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShairiStore.Models;

public class Inventory
{
    [Key]
    public int InventoryId { get; set; }

    [Required]
    public double AvailableQuantityKgs { get; set; }

    [Required]
    public double PendingQuantityKgs { get; set; }

    [Required]
    public double TotalOrderedQuantityKgs { get; set; }

    [Required]
    [ForeignKey("OrderSubCategory")]
    public int SubCategoryId { get; set; }
    public OrderSubCategory? OrderSubCategory { get; set; }

    [Required]
    public double OneMonRate { get; set; }

    public DateTime UpdatedOn { get; set; }

    [Required]
    public string UpdatedBy { get; set; }   // FK to AspNetUsers.Id (string PK)

    [ForeignKey(nameof(UpdatedBy))]
    public ApplicationUser? User { get; set; }

    [Required]
//    [ForeignKey("Order")]
    public int? OrderId { get; set; }

    //[JsonIgnore]  // 🚀 prevents infinite loop
    //public Order? Order { get; set; }
    public int? OrderType { get; set; }


    //[Required]
    //[ForeignKey("Warehouse")]
    //public int WarehouseId { get; set; }
    //public Warehouse? Warehouse { get; set; }

}
