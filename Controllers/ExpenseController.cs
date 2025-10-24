using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin,Manager")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public ExpenseController(IExpenseRepository expenseRepository, INotificationRepository notificationRepository,IUserRepository userRepository)
    {
        _expenseRepository = expenseRepository;
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    // POST: api/Expense
    [HttpPost("createexpense")]

    public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
    {
        var user = await _userRepository.GetUserByIdAsync(expense.UpdatedBy);
        if (user == null) return NotFound(new { message = "User not found" });
        var createdExpense = await _expenseRepository.CreateExpenseAsync(expense, user);
        var notificationRequest = MapNotificationPayload(title: "Expense Record Created", content: $"Expense has been created succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/expense-details/{createdExpense.ExpenseId}", notificationBy: createdExpense.UpdatedBy, notificationFor: createdExpense.UpdatedBy, notificationType: 10, DateTime.Now, "Expense");
        var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);
        return CreatedAtAction(nameof(GetExpenseById), new { expenseId = createdExpense.ExpenseId }, createdExpense);
    }

    // GET: api/Expense
    [HttpGet("getotherexpense")]
    public async Task<IActionResult> GetAllExpenses([FromQuery] int pageNNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _expenseRepository.GetAllExpensesAsync(pageNNumber, pageSize);
        return Ok(result);
    }

    // GET: api/Expense/credit
    [HttpGet("getcreditexpense")]
    public async Task<IActionResult> GetAllCreditExpenses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _expenseRepository.GetAllCreditExpenseAsync(pageNumber, pageSize);
        return Ok(result);
    }

    // GET: api/Expense/5
    [HttpGet("getexpensebyid/{expenseId:int}")]
    public async Task<IActionResult> GetExpenseById(int expenseId)
    {
        var expense = await _expenseRepository.GetExpenseByIdAsync(expenseId);
        if (expense == null) return NotFound();

        return Ok(expense);
    }

    // PUT: api/Expense/5
    [HttpPut("updateexpense/{expenseId:int}")]
    public async Task<IActionResult> UpdateExpense(int expenseId, [FromBody] Expense expense)
    {
        if (expense == null || expense.ExpenseId != expenseId)
            return BadRequest("Expense data mismatch.");

        var user = await _userRepository.GetUserByIdAsync(expense.UpdatedBy);
        if (user == null) return NotFound(new { message = "User not found" });

        var updatedExpense = await _expenseRepository.UpdateExpenseAsync(expense, user);

        if (updatedExpense == null) return NotFound();

        var notificationRequest = MapNotificationPayload(title: "Expense Record Updated", content: $"Expense has been updated succesfully by: {user?.FullName} at: {DateTime.Now}.", redirectURL: $"/expense-details/{updatedExpense.ExpenseId}", notificationBy: updatedExpense.UpdatedBy, notificationFor: updatedExpense.UpdatedBy, notificationType: 11, DateTime.Now, "Expense");
        var notification = await _notificationRepository.CreateNotificationAsync(notificationRequest);

        return Ok(updatedExpense);
    }

    // DELETE: api/Expense/5
    [HttpDelete("deleteexpensebyid/{expenseId:int}")]
    public async Task<IActionResult> DeleteExpense(int expenseId)
    {
        var deletedExpense = await _expenseRepository.DeleteExpenseAsync(expenseId);
        if (deletedExpense == null) return NotFound();

        return Ok(deletedExpense);
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