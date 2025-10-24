using Microsoft.AspNetCore.Mvc;
using ShairiStore.Common;
using ShairiStore.Enums;
using ShairiStore.Services;

namespace ShairiStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet]
    [Produces("application/vnd.ms-excel")]
    //[Produces("application/pdf")]
    public async Task<IActionResult> Export([FromQuery] ExportTypes exportType, [FromQuery] int targetId)
    {
        var columnList = GetEntityColumns(exportType)?.Split(',').Select(c => c.Trim()).ToList();

        var fileBytes = await _exportService.ExportAsync(exportType, columnList, targetId);
        var fileName = $"{exportType}_Export_{DateTime.Now:yyyyMMddHHmmss}.xls";

        //var pdfBytes = ExcelToPdfConverter.ConvertXlsBytesToPdf(fileBytes);
        //var fileName = $"{exportType}_Export_{DateTime.Now:yyyyMMddHHmmss}.pdf";

        return File(fileBytes, "application/vnd.ms-excel", fileName);
    }

    private static string? GetEntityColumns(ExportTypes type)
    {
        return null;
    }
}
