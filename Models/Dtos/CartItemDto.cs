namespace CartFlow.Api.Models.Dtos;

public sealed record CartItemDto(
    int CartItemId,
    int ProductId,
    string CategoryName,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
