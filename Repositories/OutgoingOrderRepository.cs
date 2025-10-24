using Microsoft.EntityFrameworkCore;
using ShairiStore.Enums;
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
        outgoingOrder.OrderInvoice.PendingAmount = outgoingOrder.OrderInvoice.InvoiceAmount - outgoingOrder.OutgoingOrderPayments.AmountPaid;
        outgoingOrder.OutgoingOrderPayments.InvoiceStatus = null;
        outgoingOrder.OutgoingOrderPayments.InvoiceStatusId = outgoingOrder.OutgoingOrderPayments.AmountPaid == outgoingOrder.OrderInvoice.InvoiceAmount ? 2 : 1;
        //outgoingOrder.OutgoingOrderPayments.InvoiceStatus = null;
        //outgoingOrder.OutgoingOrderPayments.InvoiceStatusId = outgoingOrder.OrderInvoice.PendingAmount == outgoingOrder.OutgoingOrderPayments.AmountPaid ? 2 : outgoingOrder.OrderInvoice.PendingAmount > outgoingOrder.OutgoingOrderPayments.AmountPaid ? 3 : 1;
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
        // --- Helper: safe enumerables ---
        var updatedDetails = updatedOrder.OutgoingOrderDetails ??  Enumerable.Empty<OutgoingOrderDetails>();
       
        // 2) Sync OutgoingOrderDetails (add / update / remove)
        var existingDetailsById = existingOrder.OutgoingOrderDetails.ToDictionary(d => d.OutgoingOrderDetailId);

        foreach (var det in updatedDetails)
        {
            // New item (no id or id == 0) OR not found in existing -> Add
            if (det.OutgoingOrderDetailId == 0 || !existingDetailsById.TryGetValue(det.OutgoingOrderDetailId, out var existDet))
            {
                det.OrderId = orderId; // ensure FK
                existingOrder.OutgoingOrderDetails.Add(det);
            }
            else
            {
                // Update scalar props of existing detail
                _context.Entry(existDet).CurrentValues.SetValues(det);
                // remove from lookup so remaining entries are the ones to delete
                existingDetailsById.Remove(det.OutgoingOrderDetailId);
            }
        }

        // Any remaining in existingDetailsById were removed by client → delete them
        foreach (var toRemove in existingDetailsById.Values)
        {
            _context.OutgoingOrderDetails.Remove(toRemove);
        }


        //// ✅ Update OrderInvoice (single)
        //if (updatedOrder.OrderInvoice != null)
        //{
        //    var paymentsSum = updatedOrder.OutgoingOrderPayments.AmountPaid;
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
        //if (updatedOrder.OutgoingOrderPayments != null)
        //{
        //    _context.Entry(existingOrder.OutgoingOrderPayments)
        //        .CurrentValues.SetValues(updatedOrder.OutgoingOrderPayments);
        //}

        // ✅ Save changes
        await _context.SaveChangesAsync();
        return existingOrder;
    }

    //public async Task<OutgoingOrder?> UpdateOrderAsync(int orderId, OutgoingOrder updatedOrder)
    //{
    //    // defensive
    //    updatedOrder ??= new OutgoingOrder();

    //    using var transaction = await _context.Database.BeginTransactionAsync();

    //    var existingOrder = await _context.OutgoingOrders
    //        .Include(o => o.OutgoingOrderDetails)
    //        .Include(o => o.OrderInvoice)
    //        .Include(o => o.OutgoingOrderPayments)
    //        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    //    if (existingOrder == null) return null;

    //    // 1) Update top-level scalar properties (do not change PKs/navigation collections here)
    //    _context.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);

    //    // --- Helper: safe enumerables ---
    //    var updatedDetails = updatedOrder.OutgoingOrderDetails ?? new List<OutgoingOrderDetails>();
    //    var updatedPayments = updatedOrder.OutgoingOrderPayments ?? new OutgoingOrderPayment();

    //    // 2) Sync OutgoingOrderDetails (add / update / remove)
    //    var existingDetailsById = existingOrder.OutgoingOrderDetails.ToDictionary(d => d.OutgoingOrderDetailId);

    //    foreach (var det in updatedDetails)
    //    {
    //        // New item (no id or id == 0) OR not found in existing -> Add
    //        if (det.OutgoingOrderDetailId == 0 || !existingDetailsById.TryGetValue(det.OutgoingOrderDetailId, out var existDet))
    //        {
    //            det.OrderId = orderId; // ensure FK
    //            existingOrder.OutgoingOrderDetails.Add(det);
    //        }
    //        else
    //        {
    //            // Update scalar props of existing detail
    //            _context.Entry(existDet).CurrentValues.SetValues(det);
    //            // remove from lookup so remaining entries are the ones to delete
    //            existingDetailsById.Remove(det.OutgoingOrderDetailId);
    //        }
    //    }

    //    // Any remaining in existingDetailsById were removed by client → delete them
    //    foreach (var toRemove in existingDetailsById.Values)
    //    {
    //        _context.OutgoingOrderDetails.Remove(toRemove);
    //    }

    //    // 3) Sync OrderInvoice (single navigation)
    //    if (updatedOrder.OrderInvoice == null)
    //    {
    //        // Client removed invoice
    //        if (existingOrder.OrderInvoice != null)
    //        {
    //            _context.OrderInvoices.Remove(existingOrder.OrderInvoice);
    //        }
    //    }
    //    else
    //    {
    //        // compute invoice status/pending using payment sums from updated payments
    //        var paymentsSum = updatedPayments.Sum(p => p.AmountPaid);
    //        var invoiceAmount = updatedOrder.OrderInvoice.InvoiceAmount;
    //        updatedOrder.OrderInvoice.InvoiceStatusId = paymentsSum == invoiceAmount ? 2 : 1;
    //        updatedOrder.OrderInvoice.PendingAmount = invoiceAmount - paymentsSum;

    //        if (existingOrder.OrderInvoice == null)
    //        {
    //            // New invoice → attach to order
    //            updatedOrder.OrderInvoice.OrderId = orderId;
    //            existingOrder.OrderInvoice = updatedOrder.OrderInvoice;
    //        }
    //        else
    //        {
    //            // Update existing invoice values
    //            _context.Entry(existingOrder.OrderInvoice).CurrentValues.SetValues(updatedOrder.OrderInvoice);
    //        }
    //    }

    //    // 4) Sync OutgoingOrderPayments (add / update / remove)
    //    var existingPaymentsById = existingOrder.OutgoingOrderPayments.ToDictionary(p => p.OutgoingOrderPaymentId);

    //    foreach (var pay in updatedPayments)
    //    {
    //        if (pay.OutgoingOrderPaymentId == 0 || !existingPaymentsById.TryGetValue(pay.OutgoingOrderPaymentId, out var existPay))
    //        {
    //            pay.OrderId = orderId; // ensure FK
    //            existingOrder.OutgoingOrderPayments.Add(pay);
    //        }
    //        else
    //        {
    //            _context.Entry(existPay).CurrentValues.SetValues(pay);
    //            existingPaymentsById.Remove(pay.OutgoingOrderPaymentId);
    //        }
    //    }

    //    // Remove payments that client deleted
    //    foreach (var toRemove in existingPaymentsById.Values)
    //    {
    //        _context.OutgoingOrderPayments.Remove(toRemove);
    //    }

    //    // 5) Persist and commit
    //    await _context.SaveChangesAsync();
    //    await transaction.CommitAsync();

    //    // Reload (optional) to return the latest graph, or return existingOrder which is tracked and updated
    //    return existingOrder;
    //}



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
