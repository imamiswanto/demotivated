namespace Demotivated.Domain.Entities;

public class ProductVariant
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string SKU { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public int Stock { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public Product? Product { get; set; }
}