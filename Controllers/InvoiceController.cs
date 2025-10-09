using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Manager")] // Restrict to Admin & Manager
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceRepository _invoiceRepo;

    public InvoiceController(IInvoiceRepository invoiceRepo)
    {
        _invoiceRepo = invoiceRepo;
    }

    [HttpGet("ListOrderInvoices")]
    public async Task<IActionResult> ListOrderInvoices(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _invoiceRepo.GetAllInvoicesAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("GetInvoiceDetailsByInvoiceId/{invoiceId}")]
    public async Task<IActionResult> GetInvoiceDetailsByInvoiceId(int invoiceId)
    {
        var invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost("CreateInvoice")]
    public async Task<IActionResult> CreateInvoice([FromBody] OrderInvoice invoice)
    {
        var createdInvoice = await _invoiceRepo.CreateInvoiceAsync(invoice);
        return CreatedAtAction(nameof(GetInvoiceDetailsByInvoiceId), new { invoiceId = createdInvoice.InvoiceId }, createdInvoice);
    }

    [HttpPut("UpdateInvoiceDetailsByInvoiceId/{invoiceId}")]
    public async Task<IActionResult> UpdateInvoiceDetailsByInvoiceId(int invoiceId, [FromBody] OrderInvoice invoice)
    {
        var updatedInvoice = await _invoiceRepo.UpdateInvoiceAsync(invoiceId, invoice);
        if (updatedInvoice == null) return NotFound();
        return Ok(updatedInvoice);
    }

    [HttpDelete("DeleteInvoice/{invoiceId}")]
    public async Task<IActionResult> DeleteInvoice(int invoiceId)
    {
        var success = await _invoiceRepo.DeleteInvoiceAsync(invoiceId);
        if (!success) return NotFound();
        return NoContent();
    }

}