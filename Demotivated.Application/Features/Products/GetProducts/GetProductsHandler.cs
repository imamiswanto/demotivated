using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Application.Features.Products.GetProducts.DTOs;

namespace Demotivated.Application.Features.Products.GetProducts;

public class GetProductsHandler
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductResponseDto>> Handle(
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);

        return products.Select(product => new ProductResponseDto(
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
        )).ToList();
    }
}