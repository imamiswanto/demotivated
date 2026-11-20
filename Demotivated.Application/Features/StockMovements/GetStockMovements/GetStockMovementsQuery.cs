namespace Demotivated.Application.Features.StockMovements.GetStockMovements;

public record GetStockMovementsQuery(
    Guid? ProductVariantId,
    string? Type,
    DateTime? StartDate,
    DateTime? EndDate,
    int Page = 1,
    int PageSize = 20
);