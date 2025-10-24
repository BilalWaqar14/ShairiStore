using Microsoft.EntityFrameworkCore;
using ShairiStore.Common;
using ShairiStore.Enums;

namespace ShairiStore.Services;

public class ExportService : IExportService
{
    private readonly AppDbContext _dbContext;

    public ExportService(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<byte[]> ExportAsync(ExportTypes exportType, IEnumerable<string>? columns = null, int? targetId = null)
    {
        object? data = exportType switch
        {
            ExportTypes.Orders => await _dbContext.Orders
                .Include(t => t.OrderType)
                .Include(s => s.OrderStatus)
                .Include(o => o.Seller)
                .Include(o => o.Broker)
                .Include(o => o.Warehouse)
                .Include(u => u.User)
                .OrderByDescending(t => t.OrderId)
                .ToListAsync(),

            ExportTypes.OrderWithDetails => await GetOrderDetails(targetId),

            ExportTypes.Payments => await _dbContext.OrderPayments
                .Include(u => u.User)
                .Include(o => o.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.InvoiceStatus)
                .OrderByDescending (t => t.PaymentId)
                .ToListAsync(),

            ExportTypes.Invoices => await _dbContext.OrderInvoices
                .Include(u => u.User)
                .Include(o => o.Orders)
                .Include(i => i.InvoiceStatus)
                .OrderByDescending(x=> x.InvoiceId)
                .ToListAsync(),

            ExportTypes.Credits => await _dbContext.Expenses.Where(x=> x.ExpenseTypeId == (int)ExpenseTypes.Credit).Include(x=> x.ExpenseType).Include(u=> u.User).OrderByDescending(x=> x.ExpenseId).ToListAsync(),
            ExportTypes.Expenses => await _dbContext.Expenses.Where(x => x.ExpenseTypeId != (int)ExpenseTypes.Credit).Include(x => x.ExpenseType).Include(u => u.User).OrderByDescending(x => x.ExpenseId).ToListAsync(),
            ExportTypes.OutGoingOrders => await _dbContext.OutgoingOrders
                .Include(s => s.Seller)
                .Include(o => o.OrderType)
                .Include(s => s.OrderStatus)
                .Include(o => o.OutgoingOrderDetails)
                .ThenInclude(sub => sub.OrderSubCategory)
                .ThenInclude(b => b.Brand)
                .Include(x => x.OutgoingOrderDetails)
                .ThenInclude(x => x.OrderSubCategory)
                .ThenInclude(x => x.Category)
                .Include(s => s.Seller)
                .Include(w => w.Warehouse)
                .Include(u => u.User)
                .Include(o => o.OutgoingOrderPayments)
                .ThenInclude(x => x.PaymentMethod)
                .Include(o => o.OutgoingOrderPayments)
                .ThenInclude(i => i.InvoiceStatus)
                .Include(o => o.OrderInvoice)
                .ThenInclude(i => i.InvoiceStatus)
                .Where(x => x.OrderId == targetId)
                .OrderByDescending(x => x.OrderId)
                .ToListAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(exportType))
        };

        if (data is null)
            throw new InvalidOperationException("No data available for export.");

        // 🔹 Delegate Excel generation
        return StableExportServiceHelper.GenerateExportFile(data, exportType ,exportType.ToString(), columns);
    }

    private async Task<object> GetOrderDetails(int? targetId = null)
    {
        return await
        _dbContext.Orders
                .Include(b => b.Broker)
                .Include(s => s.Seller)
                .Include(o => o.OrderType)
                .Include(s => s.OrderStatus)
                .Include(o => o.OrderDetails)
                .ThenInclude(sub => sub.OrderSubCategory)
                .ThenInclude(b => b.Brand)
                .Include(x=> x.OrderDetails)
                .ThenInclude(x => x.OrderSubCategory)
                .ThenInclude(x=> x.Category)
                .Include(s => s.Seller)
                .Include(b => b.Broker)
                .Include(w => w.Warehouse)
                .Include(u => u.User)
                .Include(o => o.OrderPayments)
                .ThenInclude(x=> x.PaymentMethod)
                .Include(o => o.OrderPayments)
                .ThenInclude(i => i.InvoiceStatus)
                .Include(o => o.OrderInvoice)
                .ThenInclude(i => i.InvoiceStatus)
                .Where(x=> x.OrderId == targetId)
                .OrderByDescending(x=> x.OrderId)
                .ToListAsync();
    }
}
