using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IOrderRepository
{
    Task<PagedResult<Order>> GetAllIncomingOrdersAsync(int pageNumber, int pageSize);
    Task<PagedResult<Order>> GetAllOutgoingOrdersAsync(int pageNumber, int pageSize);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(int orderId, Order order);
    Task<bool> DeleteOrderAsync(int orderId);
    Task<IEnumerable<Order>> SearchOrdersByNameAsync(string orderName);
    Task<IEnumerable<OrderType>> GetAllOrderTypesAsync();
    Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
    Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync();
    Task<IEnumerable<OrderStatus>> GetOrderStatusAsync();
    Task<IEnumerable<InvoiceStatus>> GetInvoiceStatusAsync();
    Task<Order?> GetOrderDetailsByIdAsync(int orderId);
}