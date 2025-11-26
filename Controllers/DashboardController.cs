using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Models;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;

    public DashboardController(IDashboardRepository dashboardRepository, ISubCategoryRepository subCategoryRepository ,IInvoiceRepository invoiceRepository,IPaymentRepository paymentRepository)
    {
        _dashboardRepository = dashboardRepository;
        _subCategoryRepository = subCategoryRepository;
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
    }

    [HttpPost("FetchDashboard")]
    public async Task<ActionResult<DashboardResponse>> FetchDashboardData(DashboardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _dashboardRepository.GetDashboardDataAsync(request);
        response.Payments = await _paymentRepository.ListPaymentsAsync();
        response.Invoices = await _invoiceRepository.GetInvoicesAsync();
        response.SubCategories = await _subCategoryRepository.ListSubCategoriesAsync();
        return Ok(response);
    }
}
