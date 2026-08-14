namespace CartFlow.Api.Models.Entities;

public sealed class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
