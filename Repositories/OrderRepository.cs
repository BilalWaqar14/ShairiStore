using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Order>> GetAllOrdersAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.Orders.CountAsync();
        var orders = await _context.Orders.Include(x=> x.Seller)
                                   .Include(x=> x.Broker)
                                   .Include(x=> x.OrderType)
                                   .Include(x=> x.Warehouse)
                                   .Include(x=> x.OrderStatus)
                                   .Include(x=> x.User)
                                   .OrderByDescending(o => o.OrderId)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();


        return new PagedResult<Order>(orders, totalRecords, pageNumber, pageSize);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateOrderAsync(int orderId, Order order)
    {
        var existingOrder = await _context.Orders.FindAsync(orderId);
        if (existingOrder == null) return null;

        _context.Entry(existingOrder).CurrentValues.SetValues(order);
        await _context.SaveChangesAsync();
        return existingOrder;
    }

    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Order>> SearchOrdersByNameAsync(string orderName)
    {
        return await _context.Orders
                             .Where(o => o.OrderName.Contains(orderName))
                             .ToListAsync();
    }
}
