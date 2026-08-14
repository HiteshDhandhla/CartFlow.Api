using CartFlow.Api.Data;
using CartFlow.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Api.Services;

public sealed class CatalogService(AppDbContext dbContext) : ICatalogService
{
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync()
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.CategoryName)
            .Select(category => new CategoryDto(category.CategoryId, category.CategoryName))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        int? categoryId,
        int? productId)
    {
        if (categoryId.HasValue && !await dbContext.Categories.AnyAsync(
                category => category.CategoryId == categoryId && category.IsActive))
        {
            throw new KeyNotFoundException("Category was not found.");
        }

        var activeCartId = await dbContext.Carts
            .Where(cart => cart.CartStatus == "ACTIVE")
            .OrderByDescending(cart => cart.CartId)
            .Select(cart => (int?)cart.CartId)
            .FirstOrDefaultAsync();

        var query = dbContext.Products.AsNoTracking().Where(product => product.IsActive);
        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(product => product.ProductId == productId.Value);
        }

        return await query
            .OrderBy(product => product.Category.CategoryName)
            .ThenBy(product => product.ProductName)
            .Select(product => new ProductDto(
                product.ProductId,
                product.CategoryId,
                product.Category.CategoryName,
                product.ProductName,
                product.Price,
                activeCartId.HasValue && product.CartItems.Any(item => item.CartId == activeCartId.Value)))
            .ToListAsync();
    }
}
