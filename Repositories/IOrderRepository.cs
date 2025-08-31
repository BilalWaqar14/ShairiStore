using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IOrderRepository
{
    Task<PagedResult<Order>> GetAllOrdersAsync(int pageNumber, int pageSize);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order?> UpdateOrderAsync(int orderId, Order order);
    Task<bool> DeleteOrderAsync(int orderId);
    Task<IEnumerable<Order>> SearchOrdersByNameAsync(string orderName);
}