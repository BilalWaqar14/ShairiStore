using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using ShairiStore.Enums;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Order>> GetAllIncomingOrdersAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.Orders.Where(x => x.OrderTypeId == (int)Order_Types.Incoming).CountAsync();
        var orders = await _context.Orders.Where(x => x.OrderTypeId == (int)Order_Types.Incoming).Include(x=> x.Seller)
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

    public async Task<PagedResult<Order>> GetAllOutgoingOrdersAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.Orders.Where(x => x.OrderTypeId == (int)Order_Types.Outgoing).CountAsync();
        var orders = await _context.Orders.Where(x=> x.OrderTypeId == (int)Order_Types.Outgoing).Include(x => x.Seller)
                                   .Include(x => x.Broker)
                                   .Include(x => x.OrderType)
                                   .Include(x => x.Warehouse)
                                   .Include(x => x.OrderStatus)
                                   .Include(x => x.User)
                                   .OrderByDescending(o => o.OrderId)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();


        return new PagedResult<Order>(orders, totalRecords, pageNumber, pageSize);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.Where(x => x.OrderId == orderId)
                                   .Include(x => x.Seller)
                                   .Include(x => x.Broker)
                                   .Include(x => x.OrderType)
                                   .Include(x => x.Warehouse)
                                   .Include(x => x.OrderStatus)
                                   .Include(x => x.User).FirstOrDefaultAsync();
    }

    public async Task<Order?> GetOrderDetailsByIdAsync(int orderId)
    {
        var order = await _context.Orders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.Seller)
            .Include(o => o.Broker)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderType)
            .Include(o => o.Warehouse)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.OrderSubCategory)
                    .ThenInclude(oc => oc.Category)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.OrderSubCategory)
                    .ThenInclude(b => b.Brand)
            .Include(o => o.OrderPayments)
                .ThenInclude(op => op.PaymentMethod)
            .Include(op => op.OrderPayments)
                .ThenInclude(stat => stat.InvoiceStatus)
            .Include(o => o.OrderInvoice)
                .ThenInclude(o => o.InvoiceStatus)
            .FirstOrDefaultAsync();

        if (order != null)
        {
            order.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.OrderBy);

            order.OrderDetails.ToList().ForEach(d => d.User = order.User);
            order.OrderPayments.ToList().ForEach(p => p.User = order.User);
            return order;
        }
        return null;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        order.OrderInvoice.InvoiceStatusId = 1;
        order.OrderInvoice.InvoiceStatus = null;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> UpdateOrderAsync(int orderId, Order updatedOrder)
    {
        var existingOrder = await _context.Orders
            .Include(o => o.OrderDetails)
            .Include(o => o.OrderInvoice)
            .Include(o => o.OrderPayments)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (existingOrder == null) return null;

        // ✅ Update Order main entity
        _context.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);

        // ✅ Update OrderDetails (sync add/update/remove)
        foreach (var detail in updatedOrder.OrderDetails)
        {
            detail.OrderId = orderId;
            var existingDetail = existingOrder.OrderDetails
                .FirstOrDefault(d => d.OrderDetailId == detail.OrderDetailId);

            if (existingDetail == null)
            {
                // New detail → Add
                existingOrder.OrderDetails.Add(detail);
            }
            else
            {
                // Existing detail → Update
                _context.Entry(existingDetail).CurrentValues.SetValues(detail);
            }
        }

        // Remove deleted details
        foreach (var existingDetail in existingOrder.OrderDetails.ToList())
        {
            if (!updatedOrder.OrderDetails.Any(d => d.OrderDetailId == existingDetail.OrderDetailId))
            {
                _context.OrderDetails.Remove(existingDetail);
            }
        }

        // ✅ Update OrderInvoice (single)
        if (updatedOrder.OrderInvoice != null)
        {
            var paymentsSum = updatedOrder.OrderPayments.Where(x => x.OrderId == orderId).Sum(x => x.AmountPaid);
            var invoiceAmount = updatedOrder.OrderInvoice.InvoiceAmount;
            updatedOrder.OrderInvoice.InvoiceStatusId = paymentsSum == invoiceAmount ? 2 : 1;
            updatedOrder.OrderInvoice.PendingAmount = invoiceAmount - paymentsSum;
            if (existingOrder.OrderInvoice == null)
            {
                existingOrder.OrderInvoice = updatedOrder.OrderInvoice;
            }
            else
            {
                _context.Entry(existingOrder.OrderInvoice)
                    .CurrentValues.SetValues(updatedOrder.OrderInvoice);
            }
        }

        // ✅ Update OrderPayments (sync add/update/remove)
        foreach (var payment in updatedOrder.OrderPayments)
        {
            var existingPayment = existingOrder.OrderPayments
                .FirstOrDefault(p => p.PaymentId == payment.PaymentId);

            if (existingPayment == null)
            {
                // New payment → Add
                existingOrder.OrderPayments.Add(payment);
            }
            else
            {
                // Existing payment → Update
                _context.Entry(existingPayment).CurrentValues.SetValues(payment);
            }
        }

        // Remove deleted payments
        foreach (var existingPayment in existingOrder.OrderPayments.ToList())
        {
            if (!updatedOrder.OrderPayments.Any(p => p.PaymentId == existingPayment.PaymentId))
            {
                _context.OrderPayments.Remove(existingPayment);
            }
        }

        // ✅ Save changes
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

    public async Task<IEnumerable<OrderType>> GetAllOrderTypesAsync()
    {
        return await _context.OrderTypes
                             .Where(o => o.IsActive == true)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
    {
        return await _context.Warehouses
                             .Where(o => o.IsActive == true)
                             .ToListAsync();
    }

    public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
    {
        return await _context.PaymentMethods
                             .Where(o => o.IsActive == true)
                             .ToListAsync();
    }

    public async Task<IEnumerable<OrderStatus>> GetOrderStatusAsync()
    {
        return await _context.OrderStatuses
                             .Where(o => o.IsActive == true)
                             .ToListAsync();
    }

    public async Task<IEnumerable<InvoiceStatus>> GetInvoiceStatusAsync()
    {
        return await _context.InvoiceStatuses
                             .Where(o => o.IsActive == true)
                             .ToListAsync();
    }
}
