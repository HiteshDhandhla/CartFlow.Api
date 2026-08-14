using CartFlow.Api.Models.Dtos;

namespace CartFlow.Api.Services;

public interface IInvoiceService
{
    Task<InvoiceDto> GenerateAsync();
    Task<InvoiceDto> GetByIdAsync(int invoiceId);
    Task<InvoiceDto> GetByNumberAsync(string invoiceNo);
}
