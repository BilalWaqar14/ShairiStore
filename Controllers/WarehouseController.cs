using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;


[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepo;

    public WarehouseController(IWarehouseRepository warehouseRepo)
    {
        _warehouseRepo = warehouseRepo;
    }

    [HttpGet("getallwarehouses")]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAllWarehouses()
    {
        var warehouses = await _warehouseRepo.GetAllAsync();
        return Ok(warehouses);
    }

    [HttpGet("getwarehousebyid/{id:int}")]
    public async Task<ActionResult<Warehouse>> GetWarehouseById(int id)
    {
        var warehouse = await _warehouseRepo.GetByIdAsync(id);
        if (warehouse == null)
            return NotFound();

        return Ok(warehouse);
    }

    [HttpPost("createwarehouse")]
    public async Task<ActionResult<Warehouse>> CreateWarehouse(Warehouse warehouse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _warehouseRepo.AddAsync(warehouse);
        return CreatedAtAction(nameof(GetWarehouseById), new { id = created.WarehouseId }, created);
    }

    [HttpPut("updatewarehouse/{id:int}")]
    public async Task<ActionResult<Warehouse>> UpdateWarehouse(int id, Warehouse warehouse)
    {
        if (id != warehouse.WarehouseId)
            return BadRequest("ID mismatch");

        var updated = await _warehouseRepo.UpdateAsync(warehouse);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("deletewarehouse/{id:int}")]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        var deleted = await _warehouseRepo.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}