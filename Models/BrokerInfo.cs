using System.ComponentModel.DataAnnotations;

namespace ShairiStore.Models;

public class BrokerInfo
{
    [Key]
    public int BrokerId { get; set; }

    [Required]
    public string BrokerName { get; set; }

    public double? BrokerCommission { get; set; }

    public bool IsActive { get; set; }
}
