using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class Warehouse
{
    [Key]
    public int WarehouseId { get; set; }

    [Required]
    public string WarehouseName { get; set; }

    public string? WarehouseAddress { get; set; }

    public bool IsActive { get; set; }
}

