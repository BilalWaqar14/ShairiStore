using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")] // Restrict to Admin & Manager
public class InventoryController : ControllerBase
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryController(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    [HttpGet("ListInventory")]
    public async Task<IActionResult> ListInventory(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _inventoryRepository.GetInventoryAsync(pageNumber, pageSize);
        return Ok(result);
    }
}
