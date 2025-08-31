using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationController(INotificationRepository notificationRepository, UserManager<ApplicationUser> userManager)
    {
        _notificationRepository = notificationRepository;
        _userManager = userManager;
    }

    [HttpPost("CreateNotification")]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationDetails model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        model.NotificationGeneratedBy = user.Id;
        var result = await _notificationRepository.CreateNotificationAsync(model);

        return Ok(result);
    }

    [HttpGet("ListNotifications")]
    public async Task<IActionResult> ListNotifications()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var notifications = await _notificationRepository.ListNotificationsAsync(user.Id);
        return Ok(notifications);
    }

    [HttpGet("DetailsNotificationById/{id}")]
    public async Task<IActionResult> DetailsNotificationById(int id)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(id);
        if (notification == null) return NotFound();

        return Ok(notification);
    }

    [HttpPut("SetNotificationIsRead/{id}")]
    public async Task<IActionResult> SetNotificationIsRead(int id)
    {
        var success = await _notificationRepository.SetNotificationIsReadAsync(id);
        if (!success) return NotFound();

        return Ok(new { message = "Notification marked as read" });
    }

    [HttpDelete("DeleteNotification/{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        var success = await _notificationRepository.DeleteNotificationAsync(id);
        if (!success) return NotFound();

        return Ok(new { message = "Notification deleted successfully" });
    }
}
