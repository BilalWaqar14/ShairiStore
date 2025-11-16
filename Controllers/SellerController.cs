using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SellerInfoController : ControllerBase
{
    private readonly ISellerRepository _sellerRepo;

    public SellerInfoController(ISellerRepository sellerRepo)
    {
        _sellerRepo = sellerRepo;
    }

    [HttpGet("getallsellers")]
    public async Task<ActionResult<IEnumerable<SellerInfo>>> GetAllSellers()
    {
        var sellers = await _sellerRepo.ListAllSellersAsync();
        return Ok(sellers);
    }

    [HttpGet("getsellerbyid/{id:int}")]
    public async Task<ActionResult<SellerInfo>> GetSellerById(int id)
    {
        var seller = await _sellerRepo.GetByIdAsync(id);
        if (seller == null)
            return NotFound();

        return Ok(seller);
    }

    [HttpPost("createseller")]
    public async Task<ActionResult<SellerInfo>> CreateSeller(SellerInfo seller)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _sellerRepo.AddAsync(seller);
        return CreatedAtAction(nameof(GetSellerById), new { id = created.SellerId }, created);
    }

    [HttpPut("updateseller/{id:int}")]
    public async Task<ActionResult<SellerInfo>> UpdateSeller(int id, SellerInfo seller)
    {
        if (id != seller.SellerId)
            return BadRequest("ID mismatch");

        var updated = await _sellerRepo.UpdateAsync(seller);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("deleteseller/{id:int}")]
    public async Task<IActionResult> DeleteSeller(int id)
    {
        var deleted = await _sellerRepo.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}