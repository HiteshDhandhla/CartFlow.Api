using CartFlow.Api.Models.Dtos;
using CartFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Api.Controllers;

[ApiController]
[Route("api/categories")]
public sealed class CategoriesController(ICatalogService catalogService, ILogger<CategoriesController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryDto>>>> GetAsync()
    {
        try
        {
            var categories = await catalogService.GetCategoriesAsync();
            return Ok(ApiResponse<IReadOnlyList<CategoryDto>>.Ok(categories));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve categories.");
            return StatusCode(500, ApiResponse<IReadOnlyList<CategoryDto>>.Fail("Unable to retrieve categories."));
        }
    }

    [HttpGet("{categoryId:int}/products")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProductDto>>>> GetProductsAsync(int categoryId)
    {
        try
        {
            var products = await catalogService.GetProductsAsync(categoryId, null);
            return Ok(ApiResponse<IReadOnlyList<ProductDto>>.Ok(products));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ApiResponse<IReadOnlyList<ProductDto>>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve products for category {CategoryId}.", categoryId);
            return StatusCode(500, ApiResponse<IReadOnlyList<ProductDto>>.Fail("Unable to retrieve products."));
        }
    }
}
