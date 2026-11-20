using Demotivated.Application.Features.StockMovements.GetStockMovements;
using Demotivated.Application.Features.StockMovements.GetStockMovements.DTOs;

namespace Demotivated.Application.Abstractions.Repositories;

public interface IStockMovementRepository
{
    Task<PagedResultDto<StockMovementDto>> GetAllAsync(
        GetStockMovementsQuery query,
        CancellationToken cancellationToken);

    Task<StockMovementDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}