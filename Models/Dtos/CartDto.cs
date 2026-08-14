namespace CartFlow.Api.Models.Dtos;

public sealed record CartDto(int? CartId, IReadOnlyList<CartItemDto> Items, decimal GrandTotal);
