using CartFlow.Api.Data;
using CartFlow.Api.Models.Dtos;
using CartFlow.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Api.Services;

public sealed class CartService(AppDbContext dbContext) : ICartService
{
    public async Task<CartDto> GetActiveCartAsync()
    {
        var cart = await dbContext.Carts
            .AsNoTracking()
            .Where(candidate => candidate.CartStatus == "ACTIVE")
            .OrderByDescending(candidate => candidate.CartId)
            .Select(candidate => new CartDto(
                candidate.CartId,
                candidate.Items
                    .OrderBy(item => item.CartItemId)
                    .Select(item => new CartItemDto(
                        item.CartItemId,
                        item.ProductId,
                        item.Product.Category.CategoryName,
                        item.Product.ProductName,
                        item.Quantity,
                        item.UnitPrice,
                        item.UnitPrice * item.Quantity))
                    .ToList(),
                candidate.Items.Sum(item => item.UnitPrice * item.Quantity)))
            .FirstOrDefaultAsync();

        return cart ?? new CartDto(null, [], 0);
    }

    public async Task<CartItemDto> AddItemAsync(AddCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        var product = await dbContext.Products
            .Include(item => item.Category)
            .SingleOrDefaultAsync(item => item.ProductId == request.ProductId && item.IsActive)
            ?? throw new KeyNotFoundException("Active product was not found.");

        var cart = await dbContext.Carts
            .Where(item => item.CartStatus == "ACTIVE")
            .OrderByDescending(item => item.CartId)
            .FirstOrDefaultAsync();

        if (cart is null)
        {
            cart = new Cart { CartStatus = "ACTIVE", CreatedAt = DateTime.UtcNow };
            dbContext.Carts.Add(cart);
        }
        else if (await dbContext.CartItems.AnyAsync(
                     item => item.CartId == cart.CartId && item.ProductId == request.ProductId))
        {
            throw new InvalidOperationException("Product already exists in the active cart.");
        }

        var cartItem = new CartItem
        {
            Cart = cart,
            ProductId = product.ProductId,
            Quantity = request.Quantity,
            UnitPrice = product.Price,
            CreatedAt = DateTime.UtcNow
        };
        dbContext.CartItems.Add(cartItem);
        await dbContext.SaveChangesAsync();

        return new CartItemDto(
            cartItem.CartItemId,
            product.ProductId,
            product.Category.CategoryName,
            product.ProductName,
            cartItem.Quantity,
            cartItem.UnitPrice,
            cartItem.UnitPrice * cartItem.Quantity);
    }

    public async Task DeleteItemAsync(int cartItemId)
    {
        var item = await dbContext.CartItems
            .Include(candidate => candidate.Cart)
            .SingleOrDefaultAsync(candidate => candidate.CartItemId == cartItemId)
            ?? throw new KeyNotFoundException("Cart item was not found.");

        if (item.Cart.CartStatus != "ACTIVE")
        {
            throw new InvalidOperationException("Only items in the active cart can be removed.");
        }

        dbContext.CartItems.Remove(item);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> GetCountAsync()
    {
        var activeCartId = await dbContext.Carts
            .Where(cart => cart.CartStatus == "ACTIVE")
            .OrderByDescending(cart => cart.CartId)
            .Select(cart => (int?)cart.CartId)
            .FirstOrDefaultAsync();
        return activeCartId.HasValue
            ? await dbContext.CartItems.CountAsync(item => item.CartId == activeCartId.Value)
            : 0;
    }
}
