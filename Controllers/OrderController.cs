using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin,Manager")]
public class OrderController : ControllerBase
{
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
