using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Application.Features.StockMovements.GetStockMovements;
using Demotivated.Application.Features.StockMovements.GetStockMovements.DTOs;
using Demotivated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demotivated.Infrastructure.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly DemotivatedDbContext _context;

    public StockMovementRepository(DemotivatedDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<StockMovementDto>> GetAllAsync(
        GetStockMovementsQuery query,
        CancellationToken cancellationToken)
    {
        var movements = _context.StockMovements
            .AsNoTracking()
            .AsQueryable();

        if (query.ProductVariantId.HasValue)
        {
            movements = movements.Where(
                x => x.ProductVariantId == query.ProductVariantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            movements = movements.Where(
                x => x.Type.ToString() == query.Type);
        }

        if (query.StartDate.HasValue)
        {
            var startDateUtc = DateTime.SpecifyKind(
                query.StartDate.Value,
                DateTimeKind.Utc);

            movements = movements.Where(
                x => x.CreatedAt >= startDateUtc);
        }

        if (query.EndDate.HasValue)
        {
            var endDateExclusive = DateTime.SpecifyKind(
                query.EndDate.Value.Date.AddDays(1),
                DateTimeKind.Utc);

            movements = movements.Where(
                x => x.CreatedAt < endDateExclusive);
        }

        var totalItems = await movements.CountAsync(cancellationToken);

        var items = await movements
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new StockMovementDto
            {
                Id = x.Id,
                ProductName = x.ProductVariant!.Product!.Name,
                SKU = x.ProductVariant!.SKU,
                Type = x.Type.ToString(),
                Quantity = x.Quantity,
                StockBefore = x.StockBefore,
                StockAfter = x.StockAfter,
                Reference = x.Reference,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<StockMovementDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(
                (double)totalItems / query.PageSize)
        };
    }

    public async Task<StockMovementDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.StockMovements
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new StockMovementDto
            {
                Id = x.Id,
                ProductName = x.ProductVariant!.Product!.Name,
                SKU = x.ProductVariant!.SKU,
                Type = x.Type.ToString(),
                Quantity = x.Quantity,
                StockBefore = x.StockBefore,
                StockAfter = x.StockAfter,
                Reference = x.Reference,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}