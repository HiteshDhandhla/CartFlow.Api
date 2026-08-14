namespace CartFlow.Api.Models.Dtos;

public sealed record InvoiceDto(
    int InvoiceId,
    string InvoiceNo,
    DateTime OrderDate,
    DateTime InvoiceDate,
    IReadOnlyList<InvoiceItemDto> Items,
    decimal GrandTotal);
