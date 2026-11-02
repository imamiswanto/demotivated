namespace Demotivated.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; set; }

    public Guid ProductVariantId { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public ProductVariant? ProductVariant { get; set; }
}