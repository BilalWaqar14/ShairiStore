namespace ShairiStore.Models;

public class DashboardResponse
{
    public DashboardCard IncomingOrderInfo { get; set; }
    public DashboardCard OutgoingOrderInfo { get; set; }
    public DashboardCard PaymentInfo { get; set; }
    public DashboardCard InvoiceInfo { get; set; }
    public DashboardCard CreditInfo { get; set; }
    public DashboardCard ExpenseInfo { get; set; }
    public IList<double>? PaidAmounts { get; set; }
    public IList<double>? PendingAmounts { get; set; }
    public IList<OrderSubCategory> SubCategories { get; set; }
    public IList<OrderInvoice>  Invoices { get; set; }
    public IList<OrderPayment> Payments { get; set; }
    public IList<double>? PieChartData { get; set; }
}

public class DashboardRequest
{
    public int DashboardTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class DashboardCard
{
    public int TotalCount { get; set; }
    public int ReceivedCount { get; set; }
    public int PendingCount { get; set; }
}
