using System.ComponentModel.DataAnnotations;

namespace CartFlow.Api.Models.Dtos;

public sealed class AddCartItemRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}
