using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;
using Demotivated.Domain.Enums;
using Demotivated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demotivated.Infrastructure.Repositories;

public class StockAdjustmentRepository : IStockAdjustmentRepository
{
    private readonly DemotivatedDbContext _context;

    public StockAdjustmentRepository(DemotivatedDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovement> CreateAdjustmentAsync(
        Guid productVariantId,
        int physicalStock,
        string notes,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // Lock ProductVariant selama transaksi berlangsung.
            var variant = await _context.ProductVariants
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "ProductVariants"
                    WHERE "Id" = {productVariantId}
                    FOR UPDATE
                    """)
                .FirstOrDefaultAsync(cancellationToken);

            if (variant is null)
            {
                throw new KeyNotFoundException(
                    "Product variant was not found.");
            }

            if (!variant.IsActive)
            {
                throw new InvalidOperationException(
                    "Cannot adjust stock for an inactive variant.");
            }

            if (physicalStock < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(physicalStock),
                    "Physical stock cannot be negative.");
            }

            var stockBefore = variant.Stock;
            var quantityLong = (long)physicalStock - stockBefore;

            if (quantityLong == 0)
            {
                throw new InvalidOperationException(
                    "No stock adjustment is required.");
            }

            if (quantityLong < int.MinValue || quantityLong > int.MaxValue)
            {
                throw new InvalidOperationException(
                    "Stock adjustment quantity is out of range.");
            }
            var quantity = (int)quantityLong;

            // Update stok variant.
            variant.Stock = physicalStock;

            // Buat catatan StockMovement.
            var movement = new StockMovement
            {
                Id = Guid.NewGuid(),
                ProductVariantId = variant.Id,
                Type = StockMovementType.Adjustment,
                Quantity = quantity,
                StockBefore = stockBefore,
                StockAfter = physicalStock,
                ReferenceType = "ManualAdjustment",
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            await _context.StockMovements.AddAsync(
                movement,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return movement;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
