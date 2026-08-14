using CartFlow.Api.Models.Dtos;
using CartFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Api.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GenerateAsync()
    {
        try
        {
            var invoice = await invoiceService.GenerateAsync();
            return StatusCode(201, ApiResponse<InvoiceDto>.Ok(invoice, "Invoice generated successfully."));
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(ApiResponse<InvoiceDto>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to generate invoice.");
            return StatusCode(500, ApiResponse<InvoiceDto>.Fail("Unable to generate invoice."));
        }
    }

    [HttpGet("{invoiceId:int}")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetByIdAsync(int invoiceId)
    {
        try
        {
            return Ok(ApiResponse<InvoiceDto>.Ok(await invoiceService.GetByIdAsync(invoiceId)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ApiResponse<InvoiceDto>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve invoice {InvoiceId}.", invoiceId);
            return StatusCode(500, ApiResponse<InvoiceDto>.Fail("Unable to retrieve invoice."));
        }
    }

    [HttpGet("by-number/{invoiceNo}")]
    public async Task<ActionResult<ApiResponse<InvoiceDto>>> GetByNumberAsync(string invoiceNo)
    {
        try
        {
            return Ok(ApiResponse<InvoiceDto>.Ok(await invoiceService.GetByNumberAsync(invoiceNo)));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ApiResponse<InvoiceDto>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve invoice {InvoiceNo}.", invoiceNo);
            return StatusCode(500, ApiResponse<InvoiceDto>.Fail("Unable to retrieve invoice."));
        }
    }
}
