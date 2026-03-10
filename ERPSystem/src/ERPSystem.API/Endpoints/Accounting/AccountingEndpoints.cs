using Carter;
using ERPSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.API.Endpoints.Accounting;

public class AccountingEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accounting").WithTags("Accounting");

        group.MapGet("/ats", GetAtsReport);
        group.MapGet("/iva-monthly", GetIvaMonthlyReport);
        group.MapGet("/ats-simplified", GetAtsSimplifiedReport);
    }

    private static async Task<IResult> GetAtsReport(
        ApplicationDbContext db,
        Guid tenantId,
        int year,
        int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var data = await db.Invoices
            .Where(i => i.TenantId == tenantId && i.IssueDate >= start && i.IssueDate < end)
            .Select(i => new
            {
                i.FullNumber,
                i.IssueDate,
                i.Total,
                i.TaxAmount,
                i.Status
            })
            .ToListAsync();

        return Results.Ok(new
        {
            report = "ATS",
            year,
            month,
            totalInvoices = data.Count,
            totalSales = data.Sum(x => x.Total),
            totalTax = data.Sum(x => x.TaxAmount),
            documents = data
        });
    }

    private static async Task<IResult> GetIvaMonthlyReport(
        ApplicationDbContext db,
        Guid tenantId,
        int year,
        int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var summary = await db.Invoices
            .Where(i => i.TenantId == tenantId && i.IssueDate >= start && i.IssueDate < end)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                taxableBase = g.Sum(i => i.Subtotal),
                vatCollected = g.Sum(i => i.TaxAmount),
                grossSales = g.Sum(i => i.Total)
            })
            .FirstOrDefaultAsync();

        return Results.Ok(new
        {
            report = "IVA-MENSUAL",
            year,
            month,
            taxableBase = summary?.taxableBase ?? 0,
            vatCollected = summary?.vatCollected ?? 0,
            grossSales = summary?.grossSales ?? 0
        });
    }

    private static async Task<IResult> GetAtsSimplifiedReport(
        ApplicationDbContext db,
        Guid tenantId,
        int year,
        int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var docs = await db.Invoices
            .Where(i => i.TenantId == tenantId && i.IssueDate >= start && i.IssueDate < end)
            .Select(i => new
            {
                document = i.FullNumber,
                customer = i.Customer.Name,
                amount = i.Total,
                iva = i.TaxAmount
            })
            .ToListAsync();

        return Results.Ok(new
        {
            report = "ATS-SIMPLIFICADO",
            year,
            month,
            count = docs.Count,
            details = docs
        });
    }
}
