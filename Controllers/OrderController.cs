using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Enums;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin,Manager")]
public class OrderController : ControllerBase

{
    private readonly IOrderRepository _orderRepo;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOutgoingOrderRepository _outgoingOrderRepository;
    public OrderController(IOrderRepository orderRepo, INotificationRepository notificationRepository, IUserRepository userRepository, IInventoryRepository inventoryRepository, IOutgoingOrderRepository outgoingOrderRepository)
    {
        _orderRepo = orderRepo;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _inventoryRepository = inventoryRepository;
        _outgoingOrderRepository = outgoingOrderRepository;
    }

    [HttpGet("ListAllIncomingOrders")]
    public async Task<IActionResult> ListAllIncomingOrders(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _orderRepo.GetAllIncomingOrdersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("ListAllOutgoingOrders")]
    public async Task<IActionResult> ListAllOutgoingOrders(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _outgoingOrderRepository.GetAllOutgoingOrdersAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("GetOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> GetOrderDetailsByOrderId(int orderId)
    {
        var order = await _orderRepo.GetOrderDetailsByIdAsync(orderId);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("GetOutgoingOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> GetOutgoingOrderDetailsByOrderId(int orderId)
    {
        var order = await _outgoingOrderRepository.GetOrderDetailsByIdAsync(orderId);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("GetOrderByOrderId/{orderId}")]
    public async Task<IActionResult> GetOrderByOrderId(int orderId)
    {
        var order = await _orderRepo.GetOrderByIdAsync(orderId);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("GetOutgoingOrderByOrderId/{orderId}")]
    public async Task<IActionResult> GetOutgoingOrderByOrderId(int orderId)
    {
        var order = await _outgoingOrderRepository.GetOrderByIdAsync(orderId);
        if (order == null) return NotFound();
        return Ok(order);
    }


    [HttpPost("CreateOrder")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        try
        {
            var createdOrder = await _orderRepo.CreateOrderAsync(order);
            var user = await _userRepository.GetUserByIdAsync(createdOrder.OrderBy);
            foreach (var item in createdOrder.OrderDetails) {
                var inventory = await _inventoryRepository.UpdateInventoryAsync(createdOrder.OrderId, item.SubCategoryId, user, Order_Types.Incoming);
            }
            var notificationRequest = MapNotificationPayload(title: "Order Record Created", content: $"Order has been created succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{createdOrder.OrderId}", notificationBy: createdOrder.OrderBy, notificationFor: createdOrder.OrderBy, notificationType: 3, DateTime.Now, "Order");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            notificationRequest = MapNotificationPayload(title: "Invoice Generated", content: $"Invoice has been generated succesfully for order: {createdOrder.OrderName} by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{createdOrder.OrderId}", notificationBy: createdOrder.OrderBy, notificationFor: createdOrder.OrderBy, notificationType: 5, DateTime.Now, "Invoice");
            notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            return CreatedAtAction(nameof(GetOrderDetailsByOrderId), new { orderId = createdOrder.OrderId }, createdOrder);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [HttpPost("CreateOutgoingOrder")]
    public async Task<IActionResult> CreateOutgoingOrder([FromBody] OutgoingOrder order)
    {
        try
        {
            order.SellerId = 1;
            order.TotalOrderKgs = order.OutgoingOrderDetails.Sum(x=> x.OrderKgs);
            var createdOrder = await _outgoingOrderRepository.CreateOrderAsync(order);
            var user = await _userRepository.GetUserByIdAsync(createdOrder.OrderBy);
            foreach (var item in createdOrder.OutgoingOrderDetails)
            {
                var inventory = await _inventoryRepository.UpdateInventoryAsync(createdOrder.OrderId, item.SubCategoryId, user, Order_Types.Outgoing);
            }
            var notificationRequest = MapNotificationPayload(title: "Customer Order Record Created", content: $"Customer Order has been created succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{createdOrder.OrderId}", notificationBy: createdOrder.OrderBy, notificationFor: createdOrder.OrderBy, notificationType: 3, DateTime.Now, "Order");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            notificationRequest = MapNotificationPayload(title: "Customer Invoice Generated", content: $"Customer Invoice has been generated succesfully for order: {createdOrder.OrderName} by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{createdOrder.OrderId}", notificationBy: createdOrder.OrderBy, notificationFor: createdOrder.OrderBy, notificationType: 5, DateTime.Now, "Invoice");
            notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            return CreatedAtAction(nameof(GetOrderDetailsByOrderId), new { orderId = createdOrder.OrderId }, createdOrder);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    [HttpPut("UpdateOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> UpdateOrderDetailsByOrderId(int orderId, [FromBody] Order order)
    {       
        var updatedOrder = await _orderRepo.UpdateOrderAsync(orderId, order);
        var user = await _userRepository.GetUserByIdAsync(updatedOrder.OrderBy);
        foreach (var item in order.OrderDetails)
        {
            var inventory = await _inventoryRepository.UpdateInventoryAsync(updatedOrder.OrderId, item.SubCategoryId, user, Order_Types.Incoming);
        }
        var notificationRequest = MapNotificationPayload(title: "Order Record Updated", content: $"Order has been updated succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{updatedOrder.OrderId}", notificationBy: updatedOrder.OrderBy, notificationFor: updatedOrder.OrderBy, notificationType: 4, DateTime.Now, "Order");
        var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
        if (updatedOrder.OrderPayments.Count > 0)
        {
            notificationRequest = MapNotificationPayload(title: "Payment Cleared", content: $"Payment has been made succesfully for order: {updatedOrder.OrderName} by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{updatedOrder.OrderId}", notificationBy: updatedOrder.OrderBy, notificationFor: updatedOrder.OrderBy, notificationType: 5, DateTime.Now, "Payment");
            notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
        }
        if (updatedOrder == null) return NotFound();
        return Ok(updatedOrder);
    }

    [HttpPut("UpdateOutgoingOrderDetailsByOrderId/{orderId}")]
    public async Task<IActionResult> UpdateOutgoingOrderDetailsByOrderId(int orderId, [FromBody] OutgoingOrder order)
    {
        try
        {
            order.SellerId = 1;
            order.TotalOrderKgs = order.OutgoingOrderDetails.Sum(x => x.OrderKgs);
            var updatedOrder = await _outgoingOrderRepository.UpdateOrderAsync(orderId, order);
            var user = await _userRepository.GetUserByIdAsync(updatedOrder.OrderBy);
            foreach (var item in order.OutgoingOrderDetails)
            {
                var inventory = await _inventoryRepository.UpdateInventoryAsync(updatedOrder.OrderId, item.SubCategoryId, user, Order_Types.Outgoing);
            }
            var notificationRequest = MapNotificationPayload(title: "Customer Order Record Updated", content: $"Customer Order has been updated succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{updatedOrder.OrderId}", notificationBy: updatedOrder.OrderBy, notificationFor: updatedOrder.OrderBy, notificationType: 4, DateTime.Now, "Order");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            if (updatedOrder.OutgoingOrderPayments != null)
            {
                notificationRequest = MapNotificationPayload(title: "Customer Payment Cleared", content: $"Customer Payment has been made succesfully for order: {updatedOrder.OrderName} by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/order-details/{updatedOrder.OrderId}", notificationBy: updatedOrder.OrderBy, notificationFor: updatedOrder.OrderBy, notificationType: 5, DateTime.Now, "Payment");
                notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
            }
            if (updatedOrder == null) return NotFound();
            return Ok(updatedOrder);
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    [HttpDelete("DeleteOrder/{orderId}")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var order = await _orderRepo.GetOrderByIdAsync(orderId);
        var success = await _orderRepo.DeleteOrderAsync(orderId);
        if (!success)
        {
            return NotFound();
        }
        else
        {
            var user = await _userRepository.GetUserByIdAsync(order.OrderBy);
            var notificationRequest = MapNotificationPayload(title: "Order Deleted", content: $"Order has been deleted succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: "/orders", notificationBy: order.OrderBy, notificationFor: order.OrderBy, notificationType: 8, DateTime.Now, "Order");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
        }
        return Ok("Order Deleted Successfully");
    }

    [HttpDelete("DeleteOutgoingOrder/{orderId}")]
    public async Task<IActionResult> DeleteOutgoingOrder(int orderId)
    {
        var order = await _outgoingOrderRepository.GetOrderByIdAsync(orderId);
        var success = await _outgoingOrderRepository.DeleteOrderAsync(orderId);
        if (!success)
        {
            return NotFound();
        }
        else
        {
            var user = await _userRepository.GetUserByIdAsync(order.OrderBy);
            var notificationRequest = MapNotificationPayload(title: "Customer Order Deleted", content: $"Customer Order has been deleted succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: "/orders", notificationBy: order.OrderBy, notificationFor: order.OrderBy, notificationType: 8, DateTime.Now, "Order");
            var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
        }
        return Ok("Order Deleted Successfully");
    }


    [HttpGet("SearchOrderByOrderName")]
    public async Task<IActionResult> SearchOrderByOrderName(string orderName)
    {
        var orders = await _orderRepo.SearchOrdersByNameAsync(orderName);
        return Ok(orders);
    }


    [HttpGet("SearchOutgoingOrderByOrderName")]
    public async Task<IActionResult> SearchOutgoingOrderByOrderName(string orderName)
    {
        var orders = await _outgoingOrderRepository.SearchOrdersByNameAsync(orderName);
        return Ok(orders);
    }

    private static NotificationDetails MapNotificationPayload(string title, string content, string redirectURL, string notificationBy, string notificationFor, int notificationType, DateTime notificationDate, string moduleName)
    {
        var notificationRequest = new NotificationDetails();
        notificationRequest.NotificationTitle = title;
        notificationRequest.RedirectURL = redirectURL;
        notificationRequest.NotificationRecepient = notificationFor;
        notificationRequest.NotificationGeneratedBy = notificationBy;
        notificationRequest.IsRead = false;
        notificationRequest.IsActive = true;
        notificationRequest.NotificationContent = content;
        notificationRequest.NotificationTypeId = notificationType;
        notificationRequest.NotificationCreatedAt = notificationDate;
        notificationRequest.NotificationModule = moduleName;
        return notificationRequest;
    }
}
