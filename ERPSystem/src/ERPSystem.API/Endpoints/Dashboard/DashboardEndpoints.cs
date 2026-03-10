using Carter;
using ERPSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.API.Endpoints.Dashboard;

public class DashboardEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/products-stock", async (ApplicationDbContext db, Guid tenantId) =>
        {
            var products = await db.Products
                .Where(p => p.TenantId == tenantId && p.IsActive)
                .OrderBy(p => p.StockQuantity)
                .Select(p => new
                {
                    p.Id,
                    p.Code,
                    p.Name,
                    p.StockQuantity,
                    stockStatus = p.StockQuantity <= 0
                        ? "SIN_STOCK"
                        : p.StockQuantity <= 5
                            ? "BAJO"
                            : "OK"
                })
                .ToListAsync();

            return Results.Ok(new
            {
                totalProducts = products.Count,
                lowStock = products.Count(p => p.stockStatus == "BAJO" || p.stockStatus == "SIN_STOCK"),
                items = products
            });
        });

        group.MapGet("/sales-history", async (ApplicationDbContext db, Guid tenantId, DateTime startDate, DateTime endDate) =>
        {
            var sales = await db.Invoices
                .Where(i => i.TenantId == tenantId && i.IssueDate >= startDate && i.IssueDate <= endDate)
                .GroupBy(i => i.IssueDate.Date)
                .Select(g => new
                {
                    date = g.Key,
                    invoices = g.Count(),
                    total = g.Sum(x => x.Total)
                })
                .OrderBy(x => x.date)
                .ToListAsync();

            return Results.Ok(sales);
        });
    }
}
