using Microsoft.EntityFrameworkCore;
using ShairiStore.Enums;
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


    public async Task<Inventory?> UpdateInventoryAsync(int orderId, int subCategoryId, ApplicationUser user, Order_Types orderType)
    {
        if (orderType == Order_Types.Incoming)
        {
            return await AddItemsInInventory(orderId, subCategoryId, user, orderType);
        }
        else
        {
            return await RemoveItemsFromInventory(orderId, subCategoryId, user, orderType);
        }
    }

    private async Task<Inventory?> AddItemsInInventory(int orderId, int subCategoryId, ApplicationUser user, Order_Types order_type)
    {
        var inventoryRec = await _context.inventories.Where(x => x.OrderId == orderId && x.SubCategoryId == subCategoryId && x.OrderType == (int)order_type).CountAsync();
        var order = await _context.Orders.Where(x => x.OrderId == orderId).Include(x => x.OrderDetails).ToListAsync();
        var subCategory = await _context.SubCategories.Where(x => x.SubCategoryId == subCategoryId).FirstOrDefaultAsync();
        var inventory = new Inventory();
        if (inventoryRec > 0)
        {
            var currentInventory = await _context.inventories.Where(x => x.SubCategoryId == subCategoryId && x.OrderId == orderId && x.OrderType == (int)order_type).FirstOrDefaultAsync();
            currentInventory.UpdatedOn = DateTime.Now;
            currentInventory.UpdatedBy = user.Id;
            currentInventory.User = user;
            currentInventory.SubCategoryId = subCategoryId;
            currentInventory.OrderId = orderId;
            currentInventory.OneMonRate = subCategory.OneMonRate;
            currentInventory.TotalOrderedQuantityKgs = order.Select(x => x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.RequiredQuantity * x.RequiredKgs)).FirstOrDefault();
            currentInventory.AvailableQuantityKgs = order.Select(x => x.OrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.ReceivedQuantity * x.ReceivedKgs)).FirstOrDefault() ?? 0;
            currentInventory.PendingQuantityKgs = currentInventory.TotalOrderedQuantityKgs - currentInventory.AvailableQuantityKgs;
            currentInventory.OrderType = (int)order_type;
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
            inventory.OrderType = (int)order_type;
            _context.inventories.Add(inventory);
        }
        await _context.SaveChangesAsync();
        return inventory;
    }
    private async Task<Inventory?> RemoveItemsFromInventory(int orderId, int subCategoryId, ApplicationUser user, Order_Types order_type)
    {
        var inventoryRec = await _context.inventories.Where(x => x.OrderId == orderId && x.SubCategoryId == subCategoryId && x.OrderType == (int)order_type).CountAsync();
        var order = await _context.OutgoingOrders.Where(x => x.OrderId == orderId).Include(x => x.OutgoingOrderDetails).ToListAsync();
        var subCategory = await _context.SubCategories.Where(x => x.SubCategoryId == subCategoryId).FirstOrDefaultAsync();
        var totalOrderReference = await _context.inventories.Where(x => x.SubCategoryId == subCategoryId).SumAsync(x => x.AvailableQuantityKgs);
        var inventory = new Inventory();
        if (inventoryRec > 0)
        {
            var currentInventory = await _context.inventories.Where(x => x.SubCategoryId == subCategoryId && x.OrderId == orderId && x.OrderType == (int)order_type).FirstOrDefaultAsync();
            currentInventory.UpdatedOn = DateTime.Now;
            currentInventory.UpdatedBy = user.Id;
            currentInventory.User = user;
            currentInventory.SubCategoryId = subCategoryId;
            currentInventory.OrderId = orderId;
            currentInventory.OneMonRate = subCategory.OneMonRate;
            currentInventory.TotalOrderedQuantityKgs = order.Select(x => x.OutgoingOrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.OrderKgs)).FirstOrDefault();
            currentInventory.AvailableQuantityKgs = totalOrderReference;
            currentInventory.PendingQuantityKgs = 0;
            currentInventory.OrderType = (int)order_type;
            inventory = currentInventory;
        }
        else
        {
            inventory.SubCategoryId = subCategoryId;
            inventory.UpdatedOn = DateTime.UtcNow;
            inventory.User = user;
            inventory.UpdatedBy = user.Id;
            inventory.TotalOrderedQuantityKgs = order.Select(x => x.OutgoingOrderDetails.Where(y => y.SubCategoryId == subCategoryId).Sum(x => x.OrderKgs)).FirstOrDefault();
            inventory.AvailableQuantityKgs = totalOrderReference;
            inventory.PendingQuantityKgs = 0;
            inventory.OneMonRate = subCategory.OneMonRate;
            inventory.OrderId = orderId;
            inventory.OrderType = (int)order_type;
            _context.inventories.Add(inventory);
        }
        await _context.SaveChangesAsync();
        return inventory;
    }
}
