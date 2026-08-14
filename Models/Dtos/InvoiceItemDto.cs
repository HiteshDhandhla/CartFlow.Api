namespace CartFlow.Api.Models.Dtos;

public sealed record InvoiceItemDto(
    string CategoryName,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
