using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrderPayment>> ListAllPaymentsAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.OrderPayments.CountAsync();
        var invoices = await _context.OrderPayments
                                     .Include(x=> x.PaymentMethod)
                                     .Include(x=> x.InvoiceStatus)
                                     .Include(x=> x.User)
                                     .Include(x=> x.Order)
                                     .OrderByDescending(i => i.PaymentId)
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

        return new PagedResult<OrderPayment>(invoices, totalRecords, pageNumber, pageSize);
    }
}
