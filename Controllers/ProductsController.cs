using CartFlow.Api.Models.Dtos;
using CartFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(ICatalogService catalogService, ILogger<ProductsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProductDto>>>> GetAsync(
        [FromQuery] int? categoryId,
        [FromQuery] int? productId)
    {
        try
        {
            var products = await catalogService.GetProductsAsync(categoryId, productId);
            return Ok(ApiResponse<IReadOnlyList<ProductDto>>.Ok(products));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ApiResponse<IReadOnlyList<ProductDto>>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve products.");
            return StatusCode(500, ApiResponse<IReadOnlyList<ProductDto>>.Fail("Unable to retrieve products."));
        }
    }
}
