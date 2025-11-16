using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SubCategoryController : ControllerBase
{
    private readonly ISubCategoryRepository _repository;

    public SubCategoryController(ISubCategoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("getallsubcategory")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _repository.ListAllSubCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("getallsubcategorybycategoryid/{categoryId}")]
    public async Task<IActionResult> GetSubCategoryByCategoryId(int categoryId)
    {
        var result = await _repository.ListAllSubCategoriesByCategoryIdAsync(categoryId);
        return Ok(result);
    }

    [HttpGet("getsubcategorybyid/{id}")]
    public async Task<IActionResult> GetSubCategoryById(int id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("createsubcategory")]
    public async Task<IActionResult> CreateSubCategory([FromBody] OrderSubCategory model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _repository.AddAsync(model);
        return CreatedAtAction(nameof(GetSubCategoryById), new { id = created.SubCategoryId }, created);
    }

    [HttpPut("updatesubcategory/{id}")]
    public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] OrderSubCategory model)
    {
        if (id != model.SubCategoryId)
            return BadRequest("ID mismatch");

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        await _repository.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("deletesubcategory/{id}")]
    public async Task<IActionResult> DeleteSubCategory(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
