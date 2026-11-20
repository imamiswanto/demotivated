using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Application.Features.StockMovements.GetStockMovements.DTOs;

namespace Demotivated.Application.Features.StockMovements.GetStockMovementById;

public class GetStockMovementByIdHandler
{
    private readonly IStockMovementRepository _repository;

    public GetStockMovementByIdHandler(
        IStockMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<StockMovementDto?> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Stock movement ID is required.");

        return await _repository.GetByIdAsync(
            id,
            cancellationToken);
    }
}