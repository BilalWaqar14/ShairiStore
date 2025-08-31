using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrderInvoice>> GetAllInvoicesAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.OrderInvoices.CountAsync();
        var invoices = await _context.OrderInvoices
                                     .OrderByDescending(i => i.InvoiceId)
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();

        return new PagedResult<OrderInvoice>(invoices, totalRecords, pageNumber, pageSize);
    }

    public async Task<OrderInvoice?> GetInvoiceByIdAsync(int invoiceId)
    {
        return await _context.OrderInvoices.FindAsync(invoiceId);
    }

    public async Task<OrderInvoice> CreateInvoiceAsync(OrderInvoice invoice)
    {
        _context.OrderInvoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<OrderInvoice?> UpdateInvoiceAsync(int invoiceId, OrderInvoice invoice)
    {
        var existingInvoice = await _context.OrderInvoices.FindAsync(invoiceId);
        if (existingInvoice == null) return null;

        _context.Entry(existingInvoice).CurrentValues.SetValues(invoice);
        await _context.SaveChangesAsync();
        return existingInvoice;
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId)
    {
        var invoice = await _context.OrderInvoices.FindAsync(invoiceId);
        if (invoice == null) return false;

        _context.OrderInvoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }
}