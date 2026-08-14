using CartFlow.Api.Models.Dtos;
using CartFlow.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartFlow.Api.Controllers;

[ApiController]
[Route("api/cart")]
public sealed class CartController(ICartService cartService, ILogger<CartController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetAsync()
    {
        try
        {
            return Ok(ApiResponse<CartDto>.Ok(await cartService.GetActiveCartAsync()));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve the active cart.");
            return StatusCode(500, ApiResponse<CartDto>.Fail("Unable to retrieve the active cart."));
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<CartItemDto>>> AddItemAsync(AddCartItemRequest request)
    {
        try
        {
            var item = await cartService.AddItemAsync(request);
            return StatusCode(201, ApiResponse<CartItemDto>.Ok(item, "Product added to cart successfully."));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(ApiResponse<CartItemDto>.Fail(exception.Message));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ApiResponse<CartItemDto>.Fail(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(ApiResponse<CartItemDto>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to add product {ProductId}.", request.ProductId);
            return StatusCode(500, ApiResponse<CartItemDto>.Fail("Unable to add product to the cart."));
        }
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteItemAsync(int cartItemId)
    {
        try
        {
            await cartService.DeleteItemAsync(cartItemId);
            return Ok(ApiResponse<object>.Ok(new { cartItemId }, "Product removed from cart successfully."));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ApiResponse<object>.Fail(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(ApiResponse<object>.Fail(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to delete cart item {CartItemId}.", cartItemId);
            return StatusCode(500, ApiResponse<object>.Fail("Unable to remove product from the cart."));
        }
    }

    [HttpGet("count")]
    public async Task<ActionResult<ApiResponse<object>>> GetCountAsync()
    {
        try
        {
            var count = await cartService.GetCountAsync();
            return Ok(ApiResponse<object>.Ok(new { count }));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to retrieve cart count.");
            return StatusCode(500, ApiResponse<object>.Fail("Unable to retrieve cart count."));
        }
    }
}
