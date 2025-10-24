using Microsoft.EntityFrameworkCore;
using ShairiStore.Enums;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense, ApplicationUser user)
    {
        expense.CreatedOn = DateTime.UtcNow;
        expense.RemainingAmount = expense.Amount - expense.AmountPaid;
        expense.ExpenseStatusId = expense.Amount - expense.AmountPaid == 0 ? 2 : 1;
        expense.UpdatedBy = user.Id; // assuming ApplicationUser has Id
        expense.User = user;
        expense.UpdatedOn = DateTime.UtcNow;
        expense.InitiatedBy = user.Id;
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task<Expense> DeleteExpenseAsync(int expenseId)
    {
        var expense = await _context.Expenses.FindAsync(expenseId);
        if (expense == null) return null;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task<PagedResult<Expense>> GetAllCreditExpenseAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Expenses
            .Include(e => e.ExpenseType)
            .Include(u=> u.User)
            .Include(u => u.IntiatingUser)
            .Include(s => s.ExpenseStatus)
            .Where(e => e.ExpenseTypeId == (int)ExpenseTypes.Credit) // assuming credit means negative
            .OrderByDescending(e => e.CreatedOn);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Expense>(items,totalCount,pageNumber,pageSize);
    }

    public async Task<PagedResult<Expense>> GetAllExpensesAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Expenses
            .Include(e => e.ExpenseType)
            .Include(u => u.User)
            .Include (u => u.IntiatingUser)
            .Include(s => s.ExpenseStatus)
            .Where(x => x.ExpenseTypeId != (int)ExpenseTypes.Credit)
            .OrderByDescending(e => e.CreatedOn);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Expense>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Expense> GetExpenseByIdAsync(int expenseId)
    {
        return await _context.Expenses.Include(x=> x.ExpenseType).Include(x=> x.User).Include(s=> s.ExpenseStatus).Where(e => e.ExpenseId == expenseId).FirstAsync();
    }

    public async Task<IEnumerable<ExpenseType>> GetExpenseTypes()
    {
        return await _context.ExpenseTypes.Where(x=> x.ExpenseTypeId != (int)ExpenseTypes.Credit).ToListAsync();
    }

    public async Task<Expense> UpdateExpenseAsync(Expense expense, ApplicationUser user)
    {
        var existing = await _context.Expenses.FindAsync(expense.ExpenseId);
        if (existing == null) return null;

        // update properties
        existing.AmountPaid = expense.AmountPaid;
        existing.RemainingAmount = expense.Amount - expense.AmountPaid;
        existing.ExpenseStatus = null;
        existing.ExpenseStatusId = expense.Amount - expense.AmountPaid == 0 ? 2 : 1;
        existing.ExpenseOn = expense.ExpenseOn;
        existing.ExpenseNotes = expense.ExpenseNotes;
        existing.Amount = expense.Amount;
        existing.UpdatedOn = DateTime.Now;
        existing.UpdatedBy = user.Id;
        existing.User = user;
        existing.ExpenseTypeId = expense.ExpenseTypeId;
        existing.ExpenseType = expense.ExpenseType;

        _context.Expenses.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }
}
