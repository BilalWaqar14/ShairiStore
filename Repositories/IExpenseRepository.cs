using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface IExpenseRepository
{
    Task<PagedResult<Expense>> GetAllExpensesAsync(int pageNumber = 1, int pageSize = 10);
    Task<PagedResult<Expense>> GetAllCreditExpenseAsync(int pageNumber = 1, int pageSize = 10);
    Task<Expense> CreateExpenseAsync(Expense expense, ApplicationUser user);
    Task<Expense> UpdateExpenseAsync(Expense expense, ApplicationUser user);
    Task<Expense> DeleteExpenseAsync(int expenseId);
    Task<Expense> GetExpenseByIdAsync(int expenseId);
}
