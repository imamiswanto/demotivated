namespace Demotivated.Application.Features.StockAdjustments.CreateStockAdjustment;

public record CreateStockAdjustmentCommand(
    Guid ProductVariantId,
    int PhysicalStock,
    string Notes
);
