using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IPaymentRepository
{
    Task<PagedResult<OrderPayment>> ListAllPaymentsAsync(int pageNumber, int pageSize);
    Task<List<OrderPayment>> ListPaymentsAsync();

}
