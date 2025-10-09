using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShairiStore.Models;

public class Expense
{
    [Key]
    public int ExpenseId { get; set; }

    [Required]
    public double Amount { get; set; } // This is amount which can be expense as well as credit

    [Required]
    [ForeignKey("ExpenseType")]
    public int ExpenseTypeId { get; set; }
    public ExpenseType? ExpenseType { get; set; }

    [Required]
    public DateTime UpdatedOn { get; set; }

    [Required]
    public DateTime CreatedOn { get; set; }

    [Required]
    public string UpdatedBy { get; set; }   // FK to AspNetUsers.Id (string PK)

    [ForeignKey(nameof(UpdatedBy))]
    public ApplicationUser? User { get; set; }

    [Required]
    public string ExpenseOn { get; set; }

    [Required]
    public string ExpenseNotes { get; set; }

    public string? ExpenseScreenshot { get; set; }
}

public class ExpenseType
{
    public int ExpenseTypeId { get; set; }
    public string ExpenseName { get; set; }
    public bool IsActive { get; set; }
}

public enum ExpenseTypes
{
    ShopExpense = 1,
    HomeExpense = 2,
    TransportExpense = 3,
    ElectricityExpense = 4,
    FoodExpense = 5,
    Credit = 6,
    Others = 7
}