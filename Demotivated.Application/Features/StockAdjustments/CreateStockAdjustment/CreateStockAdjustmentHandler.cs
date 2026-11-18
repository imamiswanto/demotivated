using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.StockAdjustments.CreateStockAdjustment;

public class CreateStockAdjustmentHandler
{
    private readonly IStockAdjustmentRepository _stockAdjustmentRepository;

    public CreateStockAdjustmentHandler(
        IStockAdjustmentRepository stockAdjustmentRepository)
    {
        _stockAdjustmentRepository = stockAdjustmentRepository;
    }

    public async Task<StockMovement> Handle(
        CreateStockAdjustmentCommand command,
        CancellationToken cancellationToken)
    {
        if (command.ProductVariantId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "Product variant ID cannot be empty.");
        }

        if (command.PhysicalStock < 0)
        {
            throw new InvalidOperationException(
                "Physical stock cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(command.Notes))
        {
            throw new InvalidOperationException(
                "Adjustment notes cannot be empty.");
        }

        if (command.Notes.Trim().Length > 500)
        {
            throw new InvalidOperationException(
                "Adjustment notes cannot exceed 500 characters.");
        }

        var movement = await _stockAdjustmentRepository.CreateAdjustmentAsync(
            command.ProductVariantId,
            command.PhysicalStock,
            command.Notes.Trim(),
            cancellationToken);

        return movement;
    }
}
