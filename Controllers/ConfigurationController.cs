using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin,Manager")]
public class ConfigurationController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly ISubCategoryRepository _subCategoryRepo;
    private readonly IBrandRepository _brandRepo;
    private readonly IBrokerRepository _brokerRepo;
    private readonly ISellerRepository _sellerRepo;
    private readonly IUserRepository _userRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly IInventoryRepository _inventoryRepo;
    private readonly IExpenseRepository _expenseRepository;

    public ConfigurationController(
        ICategoryRepository categoryRepo,
        ISubCategoryRepository subCategoryRepo,
        IBrandRepository brandRepo,
        IBrokerRepository brokerRepo,
        ISellerRepository sellerRepo,
        IUserRepository userRepo,
        IOrderRepository orderRepo,
        IInventoryRepository inventoryRepo,
        IExpenseRepository expenseRepository)
    {
        _categoryRepo = categoryRepo;
        _subCategoryRepo = subCategoryRepo;
        _brandRepo = brandRepo;
        _brokerRepo = brokerRepo;
        _sellerRepo = sellerRepo;
        _userRepo = userRepo;
        _orderRepo = orderRepo;
        _inventoryRepo = inventoryRepo;
        _expenseRepository = expenseRepository;
    }


    [HttpGet("getcreateorderformdata")]
    public async Task<IActionResult> GetCreateOrderFormData()
    {
        var categoriesTask = await _categoryRepo.ListAllCategoriesAsync();
        var subCategoriesTask = await _subCategoryRepo.ListAllSubCategoriesAsync();
        var brandsTask = await _brandRepo.ListAllBrandsAsync();
        var brokersTask = await _brokerRepo.ListAllBrokersAsync();
        var sellersTask = await _sellerRepo.ListAllSellersAsync();
        var warehouseTask = await _orderRepo.GetAllWarehousesAsync();
        var orderTypeTask = await _orderRepo.GetAllOrderTypesAsync();
        var invoiceStatus = await _orderRepo.GetInvoiceStatusAsync();
        var paymentMethod = await _orderRepo.GetPaymentMethodsAsync();
        var orderStatus = await _orderRepo.GetOrderStatusAsync();
        var expenseType = await _expenseRepository.GetExpenseTypes();
        var exports = new List<ExportDownloadTypes>();
        exports.Add(new ExportDownloadTypes { TypeId = 1, TypeName = "Order With Details" });
        exports.Add(new ExportDownloadTypes { TypeId = 2, TypeName = "Payments" });
        exports.Add(new ExportDownloadTypes { TypeId = 3, TypeName = "Invoices" });
        exports.Add(new ExportDownloadTypes { TypeId = 4, TypeName = "Incoming Orders" });
        exports.Add(new ExportDownloadTypes { TypeId = 5, TypeName = "Expenses" });
        exports.Add(new ExportDownloadTypes { TypeId = 6, TypeName = "Credits" });
        exports.Add(new ExportDownloadTypes { TypeId = 7, TypeName = "OutGoing Orders" });

        var model = new CreateFormModelConsolidated
        {
            Categories = categoriesTask,
            SubCategories = subCategoriesTask,
            Brands = brandsTask,
            Brokers = brokersTask,
            Sellers = sellersTask,
            Warehouses = warehouseTask,
            OrderTypes = orderTypeTask,
            InvoiceStatus = invoiceStatus,
            OrderStatus = orderStatus,
            Payment = paymentMethod,
            ExpenseTypes = expenseType,
            ExportTypes = exports
        };

        return Ok(model);
    }


    [HttpGet("getavailablequantitybyid/{subCategoryId}")]
    public async Task<IActionResult> GetCreateOrderFormData(int subCategoryId)
    {
        var availableQuantity = await _inventoryRepo.GetAvailableQuantitybyCategoryAsync(subCategoryId);
        return Ok(availableQuantity);
    }



    [HttpGet("categories")]
    public async Task<IActionResult> ListAllCategories()
    {
        var result = await _categoryRepo.ListAllCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("subcategoriesbyid/{categoryId}")]
    public async Task<IActionResult> ListSubCategoriesByCategory(int categoryId)
    {
        var result = await _subCategoryRepo.ListAllSubCategoriesByCategoryIdAsync(categoryId);
        return Ok(result);
    }

    [HttpGet("subcategories")]
    public async Task<IActionResult> ListAllSubCategories()
    {
        var result = await _subCategoryRepo.ListAllSubCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("brands")]
    public async Task<IActionResult> ListAllBrands()
    {
        var result = await _brandRepo.ListAllBrandsAsync();
        return Ok(result);
    }

    [HttpGet("brokers")]
    public async Task<IActionResult> ListAllBrokers()
    {
        var result = await _brokerRepo.ListAllBrokersAsync();
        return Ok(result);
    }

    [HttpGet("sellers")]
    public async Task<IActionResult> ListAllSellers()
    {
        var result = await _sellerRepo.ListAllSellersAsync();
        return Ok(result);
    }

    [HttpGet("warehouses")]
    public async Task<IActionResult> ListAllWarehouses()
    {
        var result = await _sellerRepo.ListAllSellersAsync();
        return Ok(result);
    }


    [HttpGet("ordertypes")]
    public async Task<IActionResult> ListAllOrderTypes()
    {
        var result = await _sellerRepo.ListAllSellersAsync();
        return Ok(result);
    }

    [HttpGet("users")]
    public async Task<IActionResult> ListAllUsers()
    {
        var result = await _userRepo.ListAllUsersAsync();
        return Ok(result);
    }

    [HttpGet("orderstatus")]
    public async Task<IActionResult> ListOrderStatus()
    {
        var result = await _orderRepo.GetOrderStatusAsync();
        return Ok(result);
    }


    [HttpGet("invoicestatus")]
    public async Task<IActionResult> ListInvoiceStatus()
    {
        var result = await _orderRepo.GetInvoiceStatusAsync();
        return Ok(result);
    }

    [HttpGet("paymentmethods")]
    public async Task<IActionResult> ListPaymentMethods()
    {
        var result = await _orderRepo.GetPaymentMethodsAsync();
        return Ok(result);
    }

    [HttpGet("userroles")]
    public async Task<IActionResult> ListUserRoles()
    {
        var roles = new List<UserRole>();
        roles.Add(new UserRole { RoleId = 1, RoleName = "Manager" });
        roles.Add(new UserRole { RoleId = 2, RoleName = "Sub Admin" });
        roles.Add(new UserRole { RoleId = 3, RoleName = "Employee" });
        return Ok(roles);
    }


    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}