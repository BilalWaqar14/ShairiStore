using ShairiStore.Models;

namespace ShairiStore.Repositories
{
    public interface IInvoiceRepository
    {
        Task<PagedResult<OrderInvoice>> GetAllInvoicesAsync(int pageNumber, int pageSize);
        Task<OrderInvoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<OrderInvoice> CreateInvoiceAsync(OrderInvoice invoice);
        Task<OrderInvoice?> UpdateInvoiceAsync(int invoiceId, OrderInvoice invoice);
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<List<OrderInvoice>> GetInvoicesAsync();
    }
}
