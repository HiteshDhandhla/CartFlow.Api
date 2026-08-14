namespace CartFlow.Api.Models.Dtos;

public sealed record ProductDto(
    int ProductId,
    int CategoryId,
    string CategoryName,
    string ProductName,
    decimal Price,
    bool IsInCart);
