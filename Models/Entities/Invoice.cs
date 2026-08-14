namespace CartFlow.Api.Models.Entities;

public sealed class Invoice
{
    public int InvoiceId { get; set; }
    public int CartId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CreatedAt { get; set; }
    public Cart Cart { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = [];
}
