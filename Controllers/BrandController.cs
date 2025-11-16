using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandController : ControllerBase
{
    private readonly IBrandRepository _brandRepository;

    public BrandController(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    [HttpGet("getallbrands")]
    public async Task<ActionResult<IEnumerable<Brand>>> GetAllBrands()
    {
        var brands = await _brandRepository.ListAllBrandsAsync();
        return Ok(brands);
    }

    [HttpGet("getbrandbyid/{id:int}")]
    public async Task<ActionResult<Brand>> GetBrandById(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
            return NotFound();

        return Ok(brand);
    }

    [HttpPost("createbrand")]
    public async Task<ActionResult<Brand>> CreateBrand(Brand brand)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _brandRepository.AddAsync(brand);
        return CreatedAtAction(nameof(GetBrandById), new { id = created.BrandId }, created);
    }

    [HttpPut("updatebrand/{id:int}")]
    public async Task<ActionResult<Brand>> UpdateBrand(int id, Brand brand)
    {
        if (id != brand.BrandId)
            return BadRequest("ID mismatch");

        var updated = await _brandRepository.UpdateAsync(brand);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("deletebrand/{id:int}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var deleted = await _brandRepository.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}