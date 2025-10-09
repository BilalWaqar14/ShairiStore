using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class OutgoingOrderRepository : IOutgoingOrderRepository
{
    private readonly AppDbContext _context;

    public OutgoingOrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<PagedResult<OutgoingOrder>> GetAllOutgoingOrdersAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.OutgoingOrders.Where(x => x.OrderTypeId == (int)Order_Types.Outgoing).CountAsync();
        var orders = await _context.OutgoingOrders.Where(x => x.OrderTypeId == (int)Order_Types.Outgoing).Include(x => x.Seller)
                                   .Include(x => x.OrderType)
                                   .Include(x => x.Warehouse)
                                   .Include(x => x.OrderStatus)
                                   .Include(x => x.User)
                                   .OrderByDescending(o => o.OrderId)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();


        return new PagedResult<OutgoingOrder>(orders, totalRecords, pageNumber, pageSize);
    }

    public async Task<OutgoingOrder?> GetOrderByIdAsync(int orderId)
    {
        return await _context.OutgoingOrders.Where(x => x.OrderId == orderId)
                                   .Include(x => x.Seller)
                                   .Include(x => x.OrderType)
                                   .Include(x => x.Warehouse)
                                   .Include(x => x.OrderStatus)
                                   .Include(x => x.User).FirstOrDefaultAsync();
    }

    public async Task<OutgoingOrder?> GetOrderDetailsByIdAsync(int orderId)
    {
        var order = await _context.OutgoingOrders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.Seller)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderType)
            .Include(o => o.Warehouse)
            .Include(o => o.OutgoingOrderDetails)
                .ThenInclude(od => od.OrderSubCategory)
                    .ThenInclude(oc => oc.Category)
            .Include(o => o.OutgoingOrderDetails)
                .ThenInclude(od => od.OrderSubCategory)
                    .ThenInclude(b => b.Brand)
            .Include(o => o.OutgoingOrderPayments)
                .ThenInclude(op => op.PaymentMethod)
            .Include(op => op.OutgoingOrderPayments)
                .ThenInclude(stat => stat.InvoiceStatus)
            .Include(o => o.OrderInvoice)
                .ThenInclude(o => o.InvoiceStatus)
            .FirstOrDefaultAsync();

        if (order != null)
        {
            order.User = await _context.Users.FirstOrDefaultAsync(x => x.Id == order.OrderBy);

            order.OutgoingOrderDetails.ToList().ForEach(d => d.User = order.User);
            order.OutgoingOrderPayments.User = order.User;
            return order;
        }
        return null;
    }

    public async Task<OutgoingOrder> CreateOrderAsync(OutgoingOrder outgoingOrder)
    {
        outgoingOrder.OrderInvoice.InvoiceStatusId = 2;
        outgoingOrder.OrderInvoice.InvoiceStatus = null;
        outgoingOrder.OutgoingOrderPayments.InvoiceStatus = null;
        outgoingOrder.OutgoingOrderPayments.InvoiceStatusId = outgoingOrder.OrderInvoice.PendingAmount == outgoingOrder.OutgoingOrderPayments.AmountPaid ? 2 : outgoingOrder.OrderInvoice.PendingAmount > outgoingOrder.OutgoingOrderPayments.AmountPaid ? 3 : 1;
        outgoingOrder.TotalOrderKgs = outgoingOrder.OutgoingOrderDetails.Sum(x => x.OrderKgs);
        _context.OutgoingOrders.Add(outgoingOrder);
        await _context.SaveChangesAsync();
        return outgoingOrder;
    }

    public async Task<OutgoingOrder?> UpdateOrderAsync(int orderId, OutgoingOrder updatedOrder)
    {
        var existingOrder = await _context.OutgoingOrders
            .Include(o => o.OutgoingOrderDetails)
            .Include(o => o.OrderInvoice)
            .Include(o => o.OutgoingOrderPayments)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (existingOrder == null) return null;

        // ✅ Update Order main entity
        _context.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);

        // ✅ Update OrderDetails (sync add/update/remove)
        foreach (var detail in updatedOrder.OutgoingOrderDetails)
        {
            detail.OrderId = orderId;
            var existingDetail = existingOrder.OutgoingOrderDetails
                .FirstOrDefault(d => d.OutgoingOrderDetailId == detail.OutgoingOrderDetailId);

            if (existingDetail == null)
            {
                // New detail → Add
                existingOrder.OutgoingOrderDetails.Add(detail);
            }
            else
            {
                // Existing detail → Update
                _context.Entry(existingDetail).CurrentValues.SetValues(detail);
            }
        }

        // Remove deleted details
        foreach (var existingDetail in existingOrder.OutgoingOrderDetails.ToList())
        {
            if (!updatedOrder.OutgoingOrderDetails.Any(d => d.OutgoingOrderDetailId == existingDetail.OutgoingOrderDetailId))
            {
                _context.OutgoingOrderDetails.Remove(existingDetail);
            }
        }

        //// ✅ Update OrderInvoice (single)
        //if (updatedOrder.OrderInvoice != null)
        //{
        //    var paymentsSum = updatedOrder.OrderPayments.Where(x => x.OrderId == orderId).Sum(x => x.AmountPaid);
        //    var invoiceAmount = updatedOrder.OrderInvoice.InvoiceAmount;
        //    updatedOrder.OrderInvoice.InvoiceStatusId = paymentsSum == invoiceAmount ? 2 : 1;
        //    updatedOrder.OrderInvoice.PendingAmount = invoiceAmount - paymentsSum;
        //    if (existingOrder.OrderInvoice == null)
        //    {
        //        existingOrder.OrderInvoice = updatedOrder.OrderInvoice;
        //    }
        //    else
        //    {
        //        _context.Entry(existingOrder.OrderInvoice)
        //            .CurrentValues.SetValues(updatedOrder.OrderInvoice);
        //    }
        //}

        //// ✅ Update OrderPayments (sync add/update/remove)
        //foreach (var payment in updatedOrder.OrderPayments)
        //{
        //    var existingPayment = existingOrder.OrderPayments
        //        .FirstOrDefault(p => p.PaymentId == payment.PaymentId);

        //    if (existingPayment == null)
        //    {
        //        // New payment → Add
        //        existingOrder.OrderPayments.Add(payment);
        //    }
        //    else
        //    {
        //        // Existing payment → Update
        //        _context.Entry(existingPayment).CurrentValues.SetValues(payment);
        //    }
        //}

        //// Remove deleted payments
        //foreach (var existingPayment in existingOrder.OrderPayments.ToList())
        //{
        //    if (!updatedOrder.OrderPayments.Any(p => p.PaymentId == existingPayment.PaymentId))
        //    {
        //        _context.OrderPayments.Remove(existingPayment);
        //    }
        //}

        // ✅ Save changes
        await _context.SaveChangesAsync();
        return existingOrder;
    }


    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _context.OutgoingOrders.FindAsync(orderId);
        if (order == null) return false;

        _context.OutgoingOrders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<OutgoingOrder>> SearchOrdersByNameAsync(string orderName)
    {
        return await _context.OutgoingOrders
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
