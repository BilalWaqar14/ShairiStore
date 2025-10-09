using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShairiStore.Repositories;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize("Admin,Manager")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepo;

    public PaymentController(IPaymentRepository paymentRepo)
    {
        _paymentRepo = paymentRepo;
    }

    [HttpGet("ListOrderPayments")]
    public async Task<IActionResult> ListOrderPayments(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _paymentRepo.ListAllPaymentsAsync(pageNumber, pageSize);
        return Ok(result);
    }
}
