namespace Demotivated.Application.Features.StockMovements.GetStockMovements.DTOs;

public class StockMovementDto
{
    public Guid Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public int StockBefore { get; set; }

    public int StockAfter { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}