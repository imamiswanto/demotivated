using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Application.Features.Products.GetProducts.DTOs;

namespace Demotivated.Application.Features.Products.GetProductById;

public class GetProductByIdHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto?> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken
        );

        if (product is null)
            return null;

        return new ProductResponseDto(
            product.Id,
            product.CollectionId,
            product.Collection?.Name,
            product.CategoryId,
            product.Category?.Name,
            product.Name,
            product.Description,
            product.Color,
            product.Price,
            product.CreatedAt,

            product.Variants.Select(variant =>
                new ProductVariantResponseDto(
                    variant.Id,
                    variant.SKU,
                    variant.Size,
                    variant.Stock
                )).ToList(),

            product.Images.Select(image =>
                new ProductImageResponseDto(
                    image.Id,
                    image.ImageUrl,
                    image.IsPrimary,
                    image.SortOrder
                )).ToList()
        );
    }
}