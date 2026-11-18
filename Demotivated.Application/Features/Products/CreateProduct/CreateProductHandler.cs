using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Products.CreateProduct;

public class CreateProductHandler
{
    private readonly IProductRepository _productRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICollectionRepository collectionRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _collectionRepository = collectionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.GetByIdAsync(
            command.CollectionId,
            cancellationToken);

        if (collection is null)
        {
            throw new InvalidOperationException(
                "Collection does not exist or is inactive.");
        }

        var category = await _categoryRepository.GetByIdAsync(
            command.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new InvalidOperationException(
                "Category does not exist or is inactive.");
        }

        var isLinked = await _categoryRepository.IsLinkedToCollectionAsync(
            command.CategoryId,
            command.CollectionId,
            cancellationToken);

        if (!isLinked)
        {
            throw new InvalidOperationException(
                "Category is not associated with the selected Collection.");
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidOperationException(
                "Product name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(command.Color))
        {
            throw new InvalidOperationException(
                "Product color cannot be empty.");
        }

        var nameExists = await _productRepository.ExistsByNameAsync(
            command.Name.Trim(),
            cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "A product with the same name already exists.");
        }

        if (command.Variants is null || command.Variants.Count == 0)
        {
            throw new InvalidOperationException(
                "A product must have at least one variant.");
        }

        var skus = command.Variants
            .Select(x => x.SKU?.Trim() ?? string.Empty)
            .ToList();

        if (skus.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException(
                "SKU cannot be empty.");
        }

        if (skus.Distinct(StringComparer.OrdinalIgnoreCase).Count()
            != skus.Count)
        {
            throw new InvalidOperationException(
                "Duplicate SKU found in product variants.");
        }

        foreach (var sku in skus)
        {
            var skuExists = await _productRepository.ExistsBySkuAsync(
                sku,
                cancellationToken);

            if (skuExists)
            {
                throw new InvalidOperationException(
                    $"SKU '{sku}' is already in use.");
            }
        }

        foreach (var variant in command.Variants)
        {
            if (variant.Stock < 0)
            {
                throw new InvalidOperationException(
                    "Stock cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(variant.Size))
            {
                throw new InvalidOperationException(
                    "Variant size cannot be empty.");
            }
        }

        if (command.Price < 0)
        {
            throw new InvalidOperationException(
                "Product price cannot be negative.");
        }

        if (command.Images is not null)
        {
            foreach (var image in command.Images)
            {
                if (string.IsNullOrWhiteSpace(image.ImageUrl))
                {
                    throw new InvalidOperationException(
                        "Image URL cannot be empty.");
                }
            }
            if (command.Images?.Count(x => x.IsPrimary) > 1)
            {
                throw new InvalidOperationException(
                    "A product can only have one primary image.");
            }
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            CollectionId = command.CollectionId,
            CategoryId = command.CategoryId,
            Name = command.Name.Trim(),
            Description = command.Description?.Trim(),
            Color = command.Color.Trim(),
            Price = command.Price,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,

            Variants = command.Variants.Select(variant =>
                new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    SKU = variant.SKU.Trim(),
                    Size = variant.Size.Trim(),
                    Stock = variant.Stock,
                    CreatedAt = DateTime.UtcNow
                }).ToList(),

            Images = (command.Images ?? new()).Select(image =>
                new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = image.ImageUrl.Trim(),
                    IsPrimary = image.IsPrimary,
                    SortOrder = image.SortOrder,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
        };

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        return product.Id;
    }
}