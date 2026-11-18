namespace Demotivated.Application.Features.Products.GetProducts.DTOs;

public record ProductVariantResponseDto(
    Guid Id,
    string SKU,
    string Size,
    int Stock
);