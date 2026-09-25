# CartFlow API

CartFlow API is the ASP.NET Core backend for the
[CartFlow Angular application](https://github.com/HiteshDhandhla/cart-flow-ui).
It owns product search, persistent cart state, price calculations, invoice
generation, and invoice history.

The API uses Entity Framework Core with SQL Server LocalDB. Prices and totals
are always calculated from database values and are never trusted from the
frontend.

## Application Workflow

```text
Angular UI
   ↓
CartFlow API
   ↓
SQL Server LocalDB

Search Products → Add Cart → View Cart → Generate Invoice → Print Invoice
```

## User Interface

### 1. Add Cart

<img width="1919" height="868" alt="CartFlow Add Cart page" src="https://github.com/user-attachments/assets/5768d29d-50c7-4195-a004-eb441a9bb93e" />

The API supplies categories, filtered products, database-backed cart status,
and trusted prices for the Add Cart page.

### 2. View Cart

<img width="1919" height="871" alt="CartFlow View Cart page" src="https://github.com/user-attachments/assets/7893734d-ab14-4d2d-84ec-006683366dda" />

The active cart endpoint returns its items, quantities, line totals, and grand
total. Delete operations remove items from the active database cart.

### 3. Invoice

<img width="1919" height="871" alt="CartFlow invoice dialog" src="https://github.com/user-attachments/assets/cba42c5d-5665-4c10-b11b-cec7aadea379" />

Invoice generation runs inside a database transaction. It copies product and
category details into invoice items, calculates the grand total, and marks the
active cart as invoiced.

### 4. Print Invoice

<img width="1296" height="863" alt="CartFlow print invoice preview" src="https://github.com/user-attachments/assets/cec0a39b-9433-499d-8307-5aa51f53df7e" />

The API returns complete invoice data for the Angular print view and browser PDF
generation.

## Technology Stack

| Technology | Usage |
| --- | --- |
| .NET 10 | ASP.NET Core Web API |
| Entity Framework Core 10 | Database access and migrations |
| SQL Server LocalDB | Development database |
| ASP.NET Core OpenAPI | OpenAPI document generation |
| Swagger UI | Interactive API documentation |

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `GET` | `/api/categories` | Get active categories |
| `GET` | `/api/categories/{categoryId}/products` | Get products for a category |
| `GET` | `/api/products` | Get or filter products and active-cart status |
| `GET` | `/api/cart` | Get the active cart and backend-calculated total |
| `GET` | `/api/cart/count` | Get the active cart item count |
| `POST` | `/api/cart/items` | Add a product and quantity to the active cart |
| `DELETE` | `/api/cart/items/{cartItemId}` | Delete an active cart item |
| `POST` | `/api/invoices` | Generate an invoice transactionally |
| `GET` | `/api/invoices/{invoiceId}` | Get an invoice by ID |
| `GET` | `/api/invoices/by-number/{invoiceNo}` | Get an invoice by invoice number |

### Product Filters

The products endpoint accepts optional query parameters:

```http
GET /api/products?categoryId=2&productId=5
```

### Add Cart Item Request

```json
{
  "productId": 5,
  "quantity": 2
}
```

The request does not accept a price. The API reads the current product price
from SQL Server.

## Standard API Response

```json
{
  "success": true,
  "message": "Request completed successfully.",
  "data": {}
}
```

Expected errors use the same response shape with `success: false` and an
appropriate HTTP status code.

## Database

The `CartFlowDb` database contains:

| Table | Purpose |
| --- | --- |
| `category_master` | Product categories |
| `product_master` | Products and trusted prices |
| `cart_master` | Active and completed carts |
| `cart_item` | Products stored in each cart |
| `invoice_master` | Invoice header and grand total |
| `invoice_item` | Historical invoice line snapshots |

The initial EF Core migration creates the schema and seeds three categories and
twelve products. Transactional cart and invoice data is created only through
the API workflow.

## Business Rules

- Quantity must be greater than zero.
- Only active products can be added.
- A product cannot be added twice to the same active cart.
- Prices and totals are calculated on the backend.
- Empty carts cannot generate invoices.
- Invoice generation is transactional.
- Invoice numbers use the `INV-yyyyMMdd-0001` format.
- Generating an invoice marks the active cart as `INVOICED`.
- A new cart cycle can add previously invoiced products again.

## Prerequisites

- .NET 10 SDK
- SQL Server Express LocalDB
- Trusted ASP.NET Core development HTTPS certificate

Trust the local certificate if required:

```bash
dotnet dev-certs https --trust
```

## Configuration

The default LocalDB connection is stored in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CartFlowDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Do not commit production credentials. Override the connection string with
environment variables or secret configuration for deployed environments.

## Run Locally

Clone the repository, restore packages, update the database, and run the API:

```bash
git clone https://github.com/Hitesh061196/CartFlow.Api.git
cd CartFlow.Api
dotnet restore
dotnet ef database update
dotnet run
```

The HTTPS development profile runs at:

- Swagger UI: [https://localhost:7042/swagger](https://localhost:7042/swagger)
- OpenAPI JSON: [https://localhost:7042/openapi/v1.json](https://localhost:7042/openapi/v1.json)

## Build

```bash
dotnet build --configuration Release
```

## Project Structure

```text
CartFlow.Api/
├── Controllers/
│   ├── CartController.cs
│   ├── CategoriesController.cs
│   ├── InvoicesController.cs
│   └── ProductsController.cs
├── Data/
│   └── AppDbContext.cs
├── Migrations/
├── Models/
│   ├── Dtos/
│   └── Entities/
├── Services/
├── Program.cs
└── appsettings.json
```

## Related Repository

- [CartFlow Angular UI](https://github.com/Hitesh061196/cart-flow-ui)

## Author

[Hitesh061196](https://github.com/Hitesh061196)
