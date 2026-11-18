using Demotivated.Domain.Entities;

namespace Demotivated.Application.Abstractions.Repositories;

public interface IStockAdjustmentRepository
{
    Task<StockMovement> CreateAdjustmentAsync(
        Guid productVariantId,
        int physicalStock,
        string notes,
        CancellationToken cancellationToken);
}
