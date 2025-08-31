using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class OrderSubCategory
{
    [Key]
    public int SubCategoryId { get; set; }

    [Required]
    public string SubCategoryName { get; set; }

    public bool IsActive { get; set; }

    [Required]
    [ForeignKey("OrderCategory")]
    public int CategoryId { get; set; }
    public OrderCategory Category { get; set; }

    [Required]
    [ForeignKey("Brand")]
    public int BrandId { get; set; }
    public Brand Brand { get; set; }

    [Required]
    public double OneMonRate { get; set; }
}
