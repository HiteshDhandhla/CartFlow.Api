using System.Data;
using CartFlow.Api.Data;
using CartFlow.Api.Models.Dtos;
using CartFlow.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Api.Services;

public sealed class InvoiceService(AppDbContext dbContext) : IInvoiceService
{
    public async Task<InvoiceDto> GenerateAsync()
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var cart = await dbContext.Carts
            .Include(item => item.Items)
            .ThenInclude(item => item.Product)
            .ThenInclude(item => item.Category)
            .Where(item => item.CartStatus == "ACTIVE")
            .OrderByDescending(item => item.CartId)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Cart is empty.");

        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        var now = DateTime.UtcNow;
        var prefix = $"INV-{now:yyyyMMdd}-";
        var sequence = await dbContext.Invoices.CountAsync(
            invoice => invoice.InvoiceNo.StartsWith(prefix)) + 1;
        var invoice = new Invoice
        {
            CartId = cart.CartId,
            InvoiceNo = $"{prefix}{sequence:0000}",
            OrderDate = cart.CreatedAt,
            InvoiceDate = now,
            CreatedAt = now,
            GrandTotal = cart.Items.Sum(item => item.UnitPrice * item.Quantity),
            Items = cart.Items.Select(item => new InvoiceItem
            {
                ProductId = item.ProductId,
                CategoryName = item.Product.Category.CategoryName,
                ProductName = item.Product.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.UnitPrice * item.Quantity
            }).ToList()
        };

        cart.CartStatus = "INVOICED";
        cart.CompletedAt = now;
        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return Map(invoice);
    }

    public async Task<InvoiceDto> GetByIdAsync(int invoiceId)
    {
        var invoice = await InvoiceQuery().SingleOrDefaultAsync(item => item.InvoiceId == invoiceId)
            ?? throw new KeyNotFoundException("Invoice was not found.");
        return Map(invoice);
    }

    public async Task<InvoiceDto> GetByNumberAsync(string invoiceNo)
    {
        var invoice = await InvoiceQuery().SingleOrDefaultAsync(item => item.InvoiceNo == invoiceNo)
            ?? throw new KeyNotFoundException("Invoice was not found.");
        return Map(invoice);
    }

    private IQueryable<Invoice> InvoiceQuery() => dbContext.Invoices.AsNoTracking().Include(item => item.Items);

    private static InvoiceDto Map(Invoice invoice) => new(
        invoice.InvoiceId,
        invoice.InvoiceNo,
        invoice.OrderDate,
        invoice.InvoiceDate,
        invoice.Items.Select(item => new InvoiceItemDto(
            item.CategoryName,
            item.ProductName,
            item.Quantity,
            item.UnitPrice,
            item.LineTotal)).ToList(),
        invoice.GrandTotal);
}
