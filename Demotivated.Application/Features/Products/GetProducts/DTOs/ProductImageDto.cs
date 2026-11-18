namespace Demotivated.Application.Features.Products.GetProducts.DTOs;

public record ProductImageResponseDto(
    Guid Id,
    string ImageUrl,
    bool IsPrimary,
    int SortOrder
);