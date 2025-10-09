using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IOutgoingOrderRepository
{
    Task<PagedResult<OutgoingOrder>> GetAllOutgoingOrdersAsync(int pageNumber, int pageSize);
    Task<OutgoingOrder?> GetOrderByIdAsync(int orderId);
    Task<OutgoingOrder> CreateOrderAsync(OutgoingOrder order);
    Task<OutgoingOrder?> UpdateOrderAsync(int orderId, OutgoingOrder order);
    Task<bool> DeleteOrderAsync(int orderId);
    Task<IEnumerable<OutgoingOrder>> SearchOrdersByNameAsync(string orderName);
    Task<IEnumerable<OrderType>> GetAllOrderTypesAsync();
    Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
    Task<IEnumerable<OrderStatus>> GetOrderStatusAsync();
    Task<IEnumerable<InvoiceStatus>> GetInvoiceStatusAsync();
    Task<OutgoingOrder?> GetOrderDetailsByIdAsync(int orderId);
}
