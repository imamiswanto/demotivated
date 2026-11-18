namespace Demotivated.Application.Features.Products.CreateProduct;

public record CreateProductCommand(
    Guid CollectionId,
    Guid CategoryId,
    string Name,
    string? Description,
    string Color,
    decimal Price,
    List<CreateProductVariantCommand> Variants,
    List<CreateProductImageCommand> Images
);

public record CreateProductVariantCommand(
    string SKU,
    string Size,
    int Stock
);

public record CreateProductImageCommand(
    string ImageUrl,
    bool IsPrimary,
    int SortOrder
);