using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryController(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }


    [HttpGet("getallcategories")]
    public async Task<ActionResult<IEnumerable<OrderCategory>>> GetAllCategories()
    {
        var categories = await _categoryRepo.ListAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("getcategorybyid/{id:int}")]
    public async Task<ActionResult<OrderCategory>> GetCategoryById(int id)
    {
        var category = await _categoryRepo.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost("createcategory")]
    public async Task<ActionResult<OrderCategory>> CreateCategory(OrderCategory category)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _categoryRepo.AddAsync(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = created.CategoryId }, created);
    }

    [HttpPut("updatecategory/{id:int}")]
    public async Task<ActionResult<OrderCategory>> UpdateCategory(int id, OrderCategory category)
    {
        if (id != category.CategoryId)
            return BadRequest("ID mismatch");

        var updated = await _categoryRepo.UpdateAsync(category);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("deletecategory/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted = await _categoryRepo.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}