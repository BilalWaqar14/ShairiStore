using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class OrderController : ControllerBase
{
    //private readonly AppDbContext _context;

    //public OrderController(AppDbContext context)
    //{
    //    _context = context;
    //}

    //// ✅ 1. List All Orders (with pagination)
    //[HttpGet("ListAllOrders")]
    //public async Task<IActionResult> ListAllOrders(PaginationSettings paginationSettings)
    //{
    //    if (paginationSettings.PageNumber <= 0) paginationSettings.PageNumber = 1;
    //    if (paginationSettings.PageSize <= 0) paginationSettings.PageSize = 10;

    //    var totalOrders = await _context.Orders.CountAsync();
    //    var orders = await _context.Orders
    //        .OrderByDescending(o => o.OrderDate)
    //        .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
    //        .Take(paginationSettings.PageSize)
    //        .ToListAsync();

    //    return Ok(new
    //    {
    //        TotalRecords = totalOrders,
    //        PageNumber = paginationSettings.PageNumber,
    //        PageSize = paginationSettings.PageSize,
    //        Data = orders
    //    });
    //}

    //// ✅ 2. Get Order Details By OrderId
    //[HttpGet("GetOrderDetailsByOrderId/{orderId}")]
    //public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
    //{
    //    var order = await _context.Orders
    //        .Include(o => o.OrderInvoice) // If invoices are related
    //        .Include(o => o.OrderDetails)    // If order has items
    //        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    //    if (order == null) return NotFound(new { Message = "Order not found." });

    //    return Ok(order);
    //}

    //// ✅ 3. Create Order
    //[HttpPost("CreateOrder")]
    //public async Task<IActionResult> CreateOrder([FromBody] Order order)
    //{
    //    if (!ModelState.IsValid) return BadRequest(ModelState);

    //    order.OrderDate = DateTime.UtcNow;
    //    _context.Orders.Add(order);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction(nameof(GetOrderDetailsByOrderId), new { orderId = order.OrderId }, order);
    //}


    //// ✅ 4. Update Order + OrderDetails By OrderId
    //[HttpPut("UpdateOrderDetailsByOrderId/{orderId}")]
    //public async Task<IActionResult> UpdateOrderDetailsByOrderId(int orderId, [FromBody] Order updatedOrder)
    //{
    //    if (orderId != updatedOrder.OrderId)
    //        return BadRequest("Order ID mismatch.");

    //    var existingOrder = await _context.Orders
    //        .Include(o => o.OrderDetails) // Include related details
    //        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    //    if (existingOrder == null)
    //        return NotFound(new { Message = "Order not found." });

    //    // ✅ Update main Order fields
    //    existingOrder.CalculatedAmount = updatedOrder.CalculatedAmount;
    //    existingOrder.OtherExpenses = updatedOrder.OtherExpenses;
    //    existingOrder.TotalAmount = updatedOrder.TotalAmount;
    //    existingOrder.OrderStatusId = updatedOrder.OrderStatusId;
    //    existingOrder.BrokerId = updatedOrder.BrokerId;
    //    existingOrder.SellerId = updatedOrder.SellerId;

    //    // ✅ Update OrderDetails (replace with new details)
    //    if (updatedOrder.OrderDetails != null && updatedOrder.OrderDetails.Any())
    //    {
    //        // Remove old details
    //        _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

    //        // Add new details
    //        foreach (var detail in updatedOrder.OrderDetails)
    //        {
    //            detail.OrderId = existingOrder.OrderId; // Ensure FK is set
    //            _context.OrderDetails.Add(detail);
    //        }
    //    }

    //    await _context.SaveChangesAsync();

    //    return Ok(new
    //    {
    //        Message = "Order and OrderDetails updated successfully.",
    //        Order = existingOrder
    //    });
    //}


    //// ✅ 5. Delete Order
    //[HttpDelete("DeleteOrder/{orderId}")]
    //public async Task<IActionResult> DeleteOrder(int orderId)
    //{
    //    var order = await _context.Orders.FindAsync(orderId);
    //    if (order == null) return NotFound(new { Message = "Order not found." });

    //    _context.Orders.Remove(order);
    //    await _context.SaveChangesAsync();

    //    return Ok(new { Message = "Order deleted successfully." });
    //}

    //// ✅ 6. Search Order By OrderName
    //[HttpGet("SearchOrderByOrderName")]
    //public async Task<IActionResult> SearchOrderByOrderName(string orderName)
    //{
    //    if (string.IsNullOrWhiteSpace(orderName))
    //        return BadRequest(new { Message = "Order name is required." });

    //    var orders = await _context.Orders
    //        .Where(o => o.OrderName.Contains(orderName))
    //        .ToListAsync();

    //    return Ok(orders);
    //}

    private readonly IOrderRepository _orderRepo;

    public OrderController(IOrderRepository orderRepo)
    {
        _orderRepo = orderRepo;
    }

    [HttpGet("ListAllOrders")]
    public async Task<IActionResult> ListAllOrders(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _orderRepo.GetAllOrdersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("GetOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
    {
        var order = await _orderRepo.GetOrderByIdAsync(orderId);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost("CreateOrder")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        var createdOrder = await _orderRepo.CreateOrderAsync(order);
        return CreatedAtAction(nameof(GetOrderDetailsByOrderId), new { orderId = createdOrder.OrderId }, createdOrder);
    }

    [HttpPut("UpdateOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> UpdateOrderDetailsByOrderId(int orderId, [FromBody] Order order)
    {
        var updatedOrder = await _orderRepo.UpdateOrderAsync(orderId, order);
        if (updatedOrder == null) return NotFound();
        return Ok(updatedOrder);
    }

    [HttpDelete("DeleteOrder/{orderId}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var success = await _orderRepo.DeleteOrderAsync(orderId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpGet("SearchOrderByOrderName")]
    public async Task<IActionResult> SearchOrderByOrderName(string orderName)
    {
        var orders = await _orderRepo.SearchOrdersByNameAsync(orderName);
        return Ok(orders);
    }
}
