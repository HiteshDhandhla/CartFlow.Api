using CartFlow.Api.Models.Dtos;

namespace CartFlow.Api.Services;

public interface ICartService
{
    Task<CartDto> GetActiveCartAsync();
    Task<CartItemDto> AddItemAsync(AddCartItemRequest request);
    Task DeleteItemAsync(int cartItemId);
    Task<int> GetCountAsync();
}
