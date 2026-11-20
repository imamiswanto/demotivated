using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Application.Features.StockMovements.GetStockMovements.DTOs;
using Demotivated.Domain.Enums;

namespace Demotivated.Application.Features.StockMovements.GetStockMovements;

public class GetStockMovementsHandler
{
    private readonly IStockMovementRepository _repository;

    public GetStockMovementsHandler(
        IStockMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<StockMovementDto>> Handle(
        GetStockMovementsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Page < 1)
            throw new ArgumentException(
                "Page must be greater than zero.");

        if (query.PageSize < 1 || query.PageSize > 100)
            throw new ArgumentException(
                "Page size must be between 1 and 100.");

        if (!string.IsNullOrWhiteSpace(query.Type))
        {
            if (!Enum.TryParse<StockMovementType>(
                    query.Type,
                    true,
                    out var movementType) ||
                !Enum.IsDefined(movementType))
            {
                throw new ArgumentException(
                    "Invalid stock movement type.");
            }

            query = query with
            {
                Type = movementType.ToString()
            };
        }

        if (query.StartDate.HasValue &&
            query.EndDate.HasValue &&
            query.StartDate.Value > query.EndDate.Value)
        {
            throw new ArgumentException(
                "Start date cannot be later than end date.");
        }

        return await _repository.GetAllAsync(
            query,
            cancellationToken);
    }
}
