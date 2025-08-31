using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
public class ConfigurationController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly ISubCategoryRepository _subCategoryRepo;
    private readonly IBrandRepository _brandRepo;
    private readonly IBrokerRepository _brokerRepo;
    private readonly ISellerRepository _sellerRepo;
    private readonly IUserRepository _userRepo;

    public ConfigurationController(
        ICategoryRepository categoryRepo,
        ISubCategoryRepository subCategoryRepo,
        IBrandRepository brandRepo,
        IBrokerRepository brokerRepo,
        ISellerRepository sellerRepo,
        IUserRepository userRepo)
    {
        _categoryRepo = categoryRepo;
        _subCategoryRepo = subCategoryRepo;
        _brandRepo = brandRepo;
        _brokerRepo = brokerRepo;
        _sellerRepo = sellerRepo;
        _userRepo = userRepo;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> ListAllCategories()
    {
        var result = await _categoryRepo.ListAllCategoriesAsync();
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

    [HttpGet("users")]
    public async Task<IActionResult> ListAllUsers()
    {
        var result = await _userRepo.ListAllUsersAsync();
        return Ok(result);
    }
}
