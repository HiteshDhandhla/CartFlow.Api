using CartFlow.Api.Models.Dtos;

namespace CartFlow.Api.Services;

public interface ICatalogService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(int? categoryId, int? productId);
}
