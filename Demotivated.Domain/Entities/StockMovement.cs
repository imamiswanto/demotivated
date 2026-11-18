using Demotivated.Domain.Enums;

namespace Demotivated.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; set; }

    public Guid ProductVariantId { get; set; }

    public StockMovementType Type { get; set; }

    public int Quantity { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public int StockBefore { get; set; }

    public int StockAfter { get; set; }

    public string? ReferenceType { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public ProductVariant? ProductVariant { get; set; }
}