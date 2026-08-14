using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CartFlow.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cart_master",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "ACTIVE"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    completed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cart_master", x => x.cart_id);
                });

            migrationBuilder.CreateTable(
                name: "category_master",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_master", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "invoice_master",
                columns: table => new
                {
                    invoice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_id = table.Column<int>(type: "int", nullable: false),
                    invoice_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    order_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    invoice_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    grand_total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_master", x => x.invoice_id);
                    table.ForeignKey(
                        name: "fk_invoice_cart",
                        column: x => x.cart_id,
                        principalTable: "cart_master",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_master",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_master", x => x.product_id);
                    table.CheckConstraint("ck_product_price", "price >= 0");
                    table.ForeignKey(
                        name: "fk_product_category",
                        column: x => x.category_id,
                        principalTable: "category_master",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invoice_item",
                columns: table => new
                {
                    invoice_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoice_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    product_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    line_total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_item", x => x.invoice_item_id);
                    table.CheckConstraint("ck_invoice_item_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "fk_invoice_item_invoice",
                        column: x => x.invoice_id,
                        principalTable: "invoice_master",
                        principalColumn: "invoice_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cart_item",
                columns: table => new
                {
                    cart_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cart_item", x => x.cart_item_id);
                    table.CheckConstraint("ck_cart_item_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "fk_cart_item_cart",
                        column: x => x.cart_id,
                        principalTable: "cart_master",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_cart_item_product",
                        column: x => x.product_id,
                        principalTable: "product_master",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "category_master",
                columns: new[] { "category_id", "category_name", "created_at", "is_active" },
                values: new object[,]
                {
                    { 1, "Stationery", new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true },
                    { 2, "Electronics", new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true },
                    { 3, "Grocery", new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true }
                });

            migrationBuilder.InsertData(
                table: "product_master",
                columns: new[] { "product_id", "category_id", "created_at", "is_active", "price", "product_name" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 20m, "Pen" },
                    { 2, 1, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 100m, "Book" },
                    { 3, 1, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 10m, "Pencil" },
                    { 4, 1, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 60m, "Notebook" },
                    { 5, 2, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 500m, "Mouse" },
                    { 6, 2, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 900m, "Keyboard" },
                    { 7, 2, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 700m, "USB Drive" },
                    { 8, 2, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 1200m, "Headphone" },
                    { 9, 3, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 80m, "Rice" },
                    { 10, 3, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 50m, "Sugar" },
                    { 11, 3, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 120m, "Tea" },
                    { 12, 3, new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, 200m, "Coffee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cart_item_product_id",
                table: "cart_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ux_cart_item_cart_id_product_id",
                table: "cart_item",
                columns: new[] { "cart_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cart_master_cart_status",
                table: "cart_master",
                column: "cart_status");

            migrationBuilder.CreateIndex(
                name: "ux_category_master_category_name",
                table: "category_master",
                column: "category_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_invoice_id",
                table: "invoice_item",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ux_invoice_master_cart_id",
                table: "invoice_master",
                column: "cart_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_invoice_master_invoice_no",
                table: "invoice_master",
                column: "invoice_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_category_id",
                table: "product_master",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ux_product_category_id_product_name",
                table: "product_master",
                columns: new[] { "category_id", "product_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cart_item");

            migrationBuilder.DropTable(
                name: "invoice_item");

            migrationBuilder.DropTable(
                name: "product_master");

            migrationBuilder.DropTable(
                name: "invoice_master");

            migrationBuilder.DropTable(
                name: "category_master");

            migrationBuilder.DropTable(
                name: "cart_master");
        }
    }
}
