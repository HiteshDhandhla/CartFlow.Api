using CartFlow.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureCategory(modelBuilder);
        ConfigureProduct(modelBuilder);
        ConfigureCart(modelBuilder);
        ConfigureCartItem(modelBuilder);
        ConfigureInvoice(modelBuilder);
        ConfigureInvoiceItem(modelBuilder);
        SeedMasterData(modelBuilder);
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Category>();
        entity.ToTable("category_master");
        entity.HasKey(x => x.CategoryId).HasName("pk_category_master");
        entity.Property(x => x.CategoryId).HasColumnName("category_id");
        entity.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(100).IsRequired();
        entity.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
        entity.HasIndex(x => x.CategoryName).IsUnique().HasDatabaseName("ux_category_master_category_name");
    }

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Product>();
        entity.ToTable("product_master", table => table.HasCheckConstraint("ck_product_price", "price >= 0"));
        entity.HasKey(x => x.ProductId).HasName("pk_product_master");
        entity.Property(x => x.ProductId).HasColumnName("product_id");
        entity.Property(x => x.CategoryId).HasColumnName("category_id");
        entity.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(150).IsRequired();
        entity.Property(x => x.Price).HasColumnName("price").HasPrecision(18, 2);
        entity.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
        entity.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId)
            .HasConstraintName("fk_product_category").OnDelete(DeleteBehavior.Restrict);
        entity.HasIndex(x => x.CategoryId).HasDatabaseName("ix_product_category_id");
        entity.HasIndex(x => new { x.CategoryId, x.ProductName }).IsUnique()
            .HasDatabaseName("ux_product_category_id_product_name");
    }

    private static void ConfigureCart(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Cart>();
        entity.ToTable("cart_master");
        entity.HasKey(x => x.CartId).HasName("pk_cart_master");
        entity.Property(x => x.CartId).HasColumnName("cart_id");
        entity.Property(x => x.CartStatus).HasColumnName("cart_status").HasMaxLength(30).HasDefaultValue("ACTIVE");
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
        entity.Property(x => x.CompletedAt).HasColumnName("completed_at");
        entity.HasIndex(x => x.CartStatus).HasDatabaseName("ix_cart_master_cart_status");
    }

    private static void ConfigureCartItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<CartItem>();
        entity.ToTable("cart_item", table => table.HasCheckConstraint("ck_cart_item_quantity", "quantity > 0"));
        entity.HasKey(x => x.CartItemId).HasName("pk_cart_item");
        entity.Property(x => x.CartItemId).HasColumnName("cart_item_id");
        entity.Property(x => x.CartId).HasColumnName("cart_id");
        entity.Property(x => x.ProductId).HasColumnName("product_id");
        entity.Property(x => x.Quantity).HasColumnName("quantity");
        entity.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2);
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
        entity.HasOne(x => x.Cart).WithMany(x => x.Items).HasForeignKey(x => x.CartId)
            .HasConstraintName("fk_cart_item_cart").OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Product).WithMany(x => x.CartItems).HasForeignKey(x => x.ProductId)
            .HasConstraintName("fk_cart_item_product").OnDelete(DeleteBehavior.Restrict);
        entity.HasIndex(x => new { x.CartId, x.ProductId }).IsUnique()
            .HasDatabaseName("ux_cart_item_cart_id_product_id");
    }

    private static void ConfigureInvoice(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Invoice>();
        entity.ToTable("invoice_master");
        entity.HasKey(x => x.InvoiceId).HasName("pk_invoice_master");
        entity.Property(x => x.InvoiceId).HasColumnName("invoice_id");
        entity.Property(x => x.CartId).HasColumnName("cart_id");
        entity.Property(x => x.InvoiceNo).HasColumnName("invoice_no").HasMaxLength(50).IsRequired();
        entity.Property(x => x.OrderDate).HasColumnName("order_date");
        entity.Property(x => x.InvoiceDate).HasColumnName("invoice_date");
        entity.Property(x => x.GrandTotal).HasColumnName("grand_total").HasPrecision(18, 2);
        entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSDATETIME()");
        entity.HasIndex(x => x.InvoiceNo).IsUnique().HasDatabaseName("ux_invoice_master_invoice_no");
        entity.HasIndex(x => x.CartId).IsUnique().HasDatabaseName("ux_invoice_master_cart_id");
        entity.HasOne(x => x.Cart).WithOne(x => x.Invoice).HasForeignKey<Invoice>(x => x.CartId)
            .HasConstraintName("fk_invoice_cart").OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureInvoiceItem(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<InvoiceItem>();
        entity.ToTable("invoice_item", table => table.HasCheckConstraint("ck_invoice_item_quantity", "quantity > 0"));
        entity.HasKey(x => x.InvoiceItemId).HasName("pk_invoice_item");
        entity.Property(x => x.InvoiceItemId).HasColumnName("invoice_item_id");
        entity.Property(x => x.InvoiceId).HasColumnName("invoice_id");
        entity.Property(x => x.ProductId).HasColumnName("product_id");
        entity.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(100);
        entity.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(150);
        entity.Property(x => x.Quantity).HasColumnName("quantity");
        entity.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2);
        entity.Property(x => x.LineTotal).HasColumnName("line_total").HasPrecision(18, 2);
        entity.HasOne(x => x.Invoice).WithMany(x => x.Items).HasForeignKey(x => x.InvoiceId)
            .HasConstraintName("fk_invoice_item_invoice").OnDelete(DeleteBehavior.Cascade);
    }

    private static void SeedMasterData(ModelBuilder modelBuilder)
    {
        var seededAt = new DateTime(2026, 8, 14, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Stationery", CreatedAt = seededAt },
            new Category { CategoryId = 2, CategoryName = "Electronics", CreatedAt = seededAt },
            new Category { CategoryId = 3, CategoryName = "Grocery", CreatedAt = seededAt });
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, CategoryId = 1, ProductName = "Pen", Price = 20, CreatedAt = seededAt },
            new Product { ProductId = 2, CategoryId = 1, ProductName = "Book", Price = 100, CreatedAt = seededAt },
            new Product { ProductId = 3, CategoryId = 1, ProductName = "Pencil", Price = 10, CreatedAt = seededAt },
            new Product { ProductId = 4, CategoryId = 1, ProductName = "Notebook", Price = 60, CreatedAt = seededAt },
            new Product { ProductId = 5, CategoryId = 2, ProductName = "Mouse", Price = 500, CreatedAt = seededAt },
            new Product { ProductId = 6, CategoryId = 2, ProductName = "Keyboard", Price = 900, CreatedAt = seededAt },
            new Product { ProductId = 7, CategoryId = 2, ProductName = "USB Drive", Price = 700, CreatedAt = seededAt },
            new Product { ProductId = 8, CategoryId = 2, ProductName = "Headphone", Price = 1200, CreatedAt = seededAt },
            new Product { ProductId = 9, CategoryId = 3, ProductName = "Rice", Price = 80, CreatedAt = seededAt },
            new Product { ProductId = 10, CategoryId = 3, ProductName = "Sugar", Price = 50, CreatedAt = seededAt },
            new Product { ProductId = 11, CategoryId = 3, ProductName = "Tea", Price = 120, CreatedAt = seededAt },
            new Product { ProductId = 12, CategoryId = 3, ProductName = "Coffee", Price = 200, CreatedAt = seededAt });
    }
}
