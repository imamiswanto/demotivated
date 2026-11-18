namespace Demotivated.Application.Features.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid CollectionId,
    Guid CategoryId,
    string Name,
    string? Description,
    string Color,
    decimal Price,
    bool IsActive,
    List<UpdateProductVariantCommand> Variants,
    List<UpdateProductImageCommand> Images
);

public record UpdateProductVariantCommand(
    Guid? Id,
    string SKU,
    string Size,
    bool IsActive
);

public record UpdateProductImageCommand(
    Guid? Id,
    string ImageUrl,
    bool IsPrimary,
    int SortOrder
);