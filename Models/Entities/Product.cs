namespace CartFlow.Api.Models.Entities;

public sealed class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = [];
}
