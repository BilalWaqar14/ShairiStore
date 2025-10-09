using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Inventory>> GetInventoryAsync(int pageNumber, int pageSize)
    {
        var totalRecords = await _context.inventories.CountAsync();
        var inventory = await _context.inventories
                                   .Include(x => x.OrderSubCategory)
                                   .ThenInclude(x=> x.Category)
                                   .Include(x=> x.OrderSubCategory)
                                   .ThenInclude(x=> x.Brand)
                                   .Include(x => x.OrderSubCategory)
                                   .Include(x => x.User)
                                   .OrderByDescending(o => o.InventoryId)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

        return new PagedResult<Inventory>(inventory, totalRecords, pageNumber, pageSize);
    }

    public async Task<double> GetAvailableQuantitybyCategoryAsync(int subCategoryId)
    {
        var totalRecords = await _context.inventories.Where(x => x.SubCategoryId == subCategoryId).SumAsync(y => y.AvailableQuantityKgs);
        return totalRecords;
    }


    public async Task<Inventory?> UpdateInventoryAsync(int orderId, int subCategoryId, ApplicationUser user)
    {
        var inventoryRec = await _context.inventories.Where(x => x.OrderId == orderId && x.SubCategoryId == subCategoryId).CountAsync();
        var order = await _context.Orders.Where(x => x.OrderId == orderId).Include(x => x.OrderDetails).ToListAsync();
        var subCategory = await _context.SubCategories.Where(x => x.SubCategoryId == subCategoryId).FirstOrDefaultAsync();
        var inventory = new Inventory();
        if (inventoryRec > 0)
        {
            var currentInventory = await _context.inventories.Where(x => x.SubCategoryId == subCategoryId && x.OrderId == orderId).FirstOrDefaultAsync();
            currentInventory.UpdatedOn = DateTime.Now;
            currentInventory.UpdatedBy = user.Id;
            currentInventory.User = user;
            currentInventory.SubCategoryId = subCategoryId;
            currentInventory.OrderId = orderId;
            currentInventory.OneMonRate = subCategory.OneMonRate;
            currentInventory.TotalOrderedQuantityKgs = order.Select(x => x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.RequiredQuantity * x.RequiredKgs)).FirstOrDefault();
            currentInventory.AvailableQuantityKgs = order.Select(x=> x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.ReceivedQuantity * x.ReceivedKgs)).FirstOrDefault() ?? 0;
            currentInventory.PendingQuantityKgs = currentInventory.TotalOrderedQuantityKgs - currentInventory.AvailableQuantityKgs;
            inventory = currentInventory;
        }
        else
        {
            inventory.SubCategoryId = subCategoryId;
            inventory.UpdatedOn = DateTime.UtcNow;
            inventory.User = user;
            inventory.UpdatedBy = user.Id;
            inventory.TotalOrderedQuantityKgs = order.Select(x => x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.RequiredQuantity * x.RequiredKgs)).FirstOrDefault();
            inventory.AvailableQuantityKgs = order.Select(x => x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.ReceivedQuantity * x.ReceivedKgs)).FirstOrDefault() ?? 0;
            inventory.PendingQuantityKgs = order.Select(x => x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.PendingQuantity * x.PendingKgs)).FirstOrDefault() ?? 0;
            inventory.OneMonRate = subCategory.OneMonRate;
            inventory.OrderId = orderId;
            _context.inventories.Add(inventory);
        }
        await _context.SaveChangesAsync();
        return inventory;
    }
}
