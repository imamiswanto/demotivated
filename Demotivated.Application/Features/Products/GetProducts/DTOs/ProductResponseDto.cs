namespace Demotivated.Application.Features.Products.GetProducts.DTOs;

public record ProductResponseDto(
    Guid Id,
    Guid CollectionId,
    string? CollectionName,
    Guid CategoryId,
    string? CategoryName,
    string Name,
    string? Description,
    string Color,
    decimal Price,
    DateTime CreatedAt,
    List<ProductVariantResponseDto> Variants,
    List<ProductImageResponseDto> Images
);