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
    //    private readonly AppDbContext _context;

    //    public InvoiceController(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    // ✅ ListOrderInvoices (with pagination)
    //    [HttpGet("ListOrderInvoices")]
    //    public async Task<IActionResult> ListOrderInvoices(PaginationSettings paginationSettings)
    //    {
    //        var invoices = await _context.OrderInvoices
    //            .Skip((paginationSettings.PageNumber - 1) * paginationSettings.PageSize)
    //            .Take(paginationSettings.PageSize)
    //            .ToListAsync();

    //        var totalRecords = await _context.OrderInvoices.CountAsync();

    //        return Ok(new
    //        {
    //            Data = invoices,
    //            TotalRecords = totalRecords,
    //            PageNumber = paginationSettings.PageNumber,
    //            PageSize = paginationSettings.PageSize
    //        });
    //    }

    //    // ✅ GetInvoiceDetailsByInvoiceId
    //    [HttpGet("GetInvoiceDetailsByInvoiceId/{invoiceId}")]
    //    public async Task<IActionResult> GetInvoiceDetailsByInvoiceId(int invoiceId)
    //    {
    //        var invoice = await _context.OrderInvoices
    //            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

    //        if (invoice == null)
    //            return NotFound(new { Message = "Invoice not found" });

    //        return Ok(invoice);
    //    }

    //    // ✅ CreateInvoice
    //    [HttpPost("CreateInvoice")]
    //    public async Task<IActionResult> CreateInvoice([FromBody] OrderInvoice invoice)
    //    {
    //        if (!ModelState.IsValid)
    //            return BadRequest(ModelState);

    //        invoice.InvoiceDate = DateTime.UtcNow;
    //        invoice.UpdatedAt = DateTime.UtcNow;

    //        await _context.OrderInvoices.AddAsync(invoice);
    //        await _context.SaveChangesAsync();

    //        return CreatedAtAction(nameof(GetInvoiceDetailsByInvoiceId),
    //            new { invoiceId = invoice.InvoiceId }, invoice);
    //    }

    //    // ✅ UpdateInvoiceDetailsByInvoiceId
    //    [HttpPut("UpdateInvoiceDetailsByInvoiceId/{invoiceId}")]
    //    public async Task<IActionResult> UpdateInvoiceDetailsByInvoiceId(int invoiceId, [FromBody] OrderInvoice invoice)
    //    {
    //        if (invoiceId != invoice.InvoiceId)
    //            return BadRequest(new { Message = "InvoiceId mismatch" });

    //        var existingInvoice = await _context.OrderInvoices.FindAsync(invoiceId);
    //        if (existingInvoice == null)
    //            return NotFound(new { Message = "Invoice not found" });

    //        // Update fields
    //        existingInvoice.OrderId = invoice.OrderId;
    //        existingInvoice.InvoiceAmount = invoice.InvoiceAmount;
    //        existingInvoice.OrderAmount = invoice.OrderAmount;
    //        existingInvoice.PendingAmount = invoice.PendingAmount;
    //        existingInvoice.PaymentMethod = invoice.PaymentMethod;
    //        existingInvoice.PaymentScreenshot = invoice.PaymentScreenshot;
    //        existingInvoice.InvoiceBy = invoice.InvoiceBy;
    //        existingInvoice.InvoiceStatus = invoice.InvoiceStatus;
    //        existingInvoice.UpdatedBy = invoice.UpdatedBy;
    //        existingInvoice.UpdatedAt = DateTime.UtcNow;

    //        _context.OrderInvoices.Update(existingInvoice);
    //        await _context.SaveChangesAsync();

    //        return Ok(existingInvoice);
    //    }

    //    // ✅ DeleteInvoice
    //    [HttpDelete("DeleteInvoice/{invoiceId}")]
    //    public async Task<IActionResult> DeleteInvoice(int invoiceId)
    //    {
    //        var invoice = await _context.OrderInvoices.FindAsync(invoiceId);
    //        if (invoice == null)
    //            return NotFound(new { Message = "Invoice not found" });

    //        _context.OrderInvoices.Remove(invoice);
    //        await _context.SaveChangesAsync();

    //        return Ok(new { Message = "Invoice deleted successfully" });
    //    }
    //}
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