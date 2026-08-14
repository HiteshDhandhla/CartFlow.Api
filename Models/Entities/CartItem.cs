namespace CartFlow.Api.Models.Entities;

public sealed class CartItem
{
    public int CartItemId { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
