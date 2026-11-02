namespace Demotivated.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Color { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Category? Category { get; set; }

    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();

    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();

}