namespace ShairiStore.Models;

public class CreateFormModelConsolidated
{
    public IEnumerable<OrderCategory> Categories { get; set; }
    public IEnumerable<OrderSubCategory> SubCategories { get; set; }
    public IEnumerable<Brand> Brands { get; set; }
    public IEnumerable<BrokerInfo> Brokers { get; set; }
    public IEnumerable<SellerInfo> Sellers { get; set; }
    public IEnumerable<Warehouse> Warehouses { get; set; }
    public IEnumerable<OrderType> OrderTypes { get; set; }
    public IEnumerable<InvoiceStatus> InvoiceStatus { get; set; }
    public IEnumerable<PaymentMethod> Payment { get; set; }
    public IEnumerable<OrderStatus> OrderStatus { get; set; }
    public IEnumerable<ExpenseType> ExpenseTypes { get; set; }
    public IEnumerable<ExportDownloadTypes> ExportTypes { get; set; }
}

public class ExportDownloadTypes
{
    public int TypeId { get; set; }

    public string TypeName { get; set; }
}