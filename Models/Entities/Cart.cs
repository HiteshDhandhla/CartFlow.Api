namespace CartFlow.Api.Models.Entities;

public sealed class Cart
{
    public int CartId { get; set; }
    public string CartStatus { get; set; } = "ACTIVE";
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ICollection<CartItem> Items { get; set; } = [];
    public Invoice? Invoice { get; set; }
}
