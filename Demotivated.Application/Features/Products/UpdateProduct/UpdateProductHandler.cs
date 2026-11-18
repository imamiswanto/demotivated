using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Products.UpdateProduct;

public class UpdateProductHandler
{
    private readonly IProductRepository _productRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductHandler(
        IProductRepository productRepository,
        ICollectionRepository collectionRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _collectionRepository = collectionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(
        Guid id,
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Ambil product, termasuk yang sedang nonaktif.
        var product = await _productRepository.GetByIdIncludingInactiveAsync(
            id,
            cancellationToken);

        if (product is null)
            throw new KeyNotFoundException("Product not found.");

        // 2. Validasi Collection.
        var collection = await _collectionRepository.GetByIdAsync(
            command.CollectionId,
            cancellationToken);

        if (collection is null)
            throw new InvalidOperationException(
                "Collection does not exist or is inactive.");

        // 3. Validasi Category.
        var category = await _categoryRepository.GetByIdAsync(
            command.CategoryId,
            cancellationToken);

        if (category is null)
            throw new InvalidOperationException(
                "Category does not exist or is inactive.");

        // 4. Validasi relasi Collection dan Category.
        var isLinked = await _categoryRepository.IsLinkedToCollectionAsync(
            command.CategoryId,
            command.CollectionId,
            cancellationToken);

        if (!isLinked)
            throw new InvalidOperationException(
                "Category is not linked to the selected collection.");

        // 5. Validasi nama produk.
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new InvalidOperationException("Product name is required.");

        var normalizedName = command.Name.Trim();

        var nameExists = await _productRepository.ExistsByNameAsync(
            normalizedName,
            id,
            cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException(
                $"Product name '{normalizedName}' is already used.");
        }

        // 6. Validasi warna.
        if (string.IsNullOrWhiteSpace(command.Color))
            throw new InvalidOperationException("Product color is required.");

        // 7. Validasi harga.
        if (command.Price < 0)
            throw new InvalidOperationException("Product price cannot be negative.");

        // 8. Validasi variants.
        if (command.Variants is null || command.Variants.Count == 0)
            throw new InvalidOperationException(
                "Product must have at least one variant.");

        // 9. Validasi SKU kosong.
        if (command.Variants.Any(x => string.IsNullOrWhiteSpace(x.SKU)))
            throw new InvalidOperationException("SKU is required.");

        // 10. Validasi SKU duplikat dalam request.
        var skus = command.Variants
            .Select(x => x.SKU.Trim())
            .ToList();

        if (skus.Distinct(StringComparer.OrdinalIgnoreCase).Count() != skus.Count)
            throw new InvalidOperationException(
                "Duplicate SKU found in product variants.");

        foreach (var sku in skus)
        {
            var exists = await _productRepository.ExistsBySkuAsync(
                sku,
                id,
                cancellationToken);

            if (exists)
                throw new InvalidOperationException(
                    $"SKU '{sku}' is already used by another product.");
        }

        // Validasi ID variant tidak boleh duplikat.
        var variantIds = command.Variants
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToList();

        if (variantIds.Distinct().Count() != variantIds.Count)
        {
            throw new InvalidOperationException(
                "Duplicate variant ID found.");
        }

        // Pastikan semua variant yang dikirim memang milik product ini.
        foreach (var variantId in variantIds)
        {
            var variantExists = product.Variants
                .Any(x => x.Id == variantId);

            if (!variantExists)
            {
                throw new InvalidOperationException(
                    $"Variant '{variantId}' does not belong to this product.");
            }
        }

        // Variant yang tidak dikirim akan tetap dipertahankan.
        // Karena itu, SKU tidak boleh bentrok dengan variant yang tidak dikirim.
        var newVariantIds = new HashSet<Guid>();

        foreach (var variantCommand in command.Variants)
        {
            var duplicateExistingVariant = product.Variants.Any(existing =>
                !variantIds.Contains(existing.Id)
                && existing.SKU.Equals(
                    variantCommand.SKU.Trim(),
                    StringComparison.OrdinalIgnoreCase));

            if (duplicateExistingVariant)
            {
                throw new InvalidOperationException(
                    $"SKU '{variantCommand.SKU}' is already used by another variant in this product.");
            }

            if (!variantCommand.IsActive && variantCommand.Id.HasValue)
            {
                var existingVariant = product.Variants
                    .First(x => x.Id == variantCommand.Id.Value);

                if (existingVariant.Stock > 0)
                {
                    throw new InvalidOperationException(
                        $"Variant '{existingVariant.SKU}' cannot be deactivated while stock is greater than zero.");
                }
            }
        }

        // 11. Validasi ukuran.
        if (command.Variants.Any(x => string.IsNullOrWhiteSpace(x.Size)))
            throw new InvalidOperationException("Variant size is required.");

        // 12. Validasi gambar.
        if (command.Images is not null)
        {
            // Validasi URL gambar.
            if (command.Images.Any(x =>
                string.IsNullOrWhiteSpace(x.ImageUrl)))
            {
                throw new InvalidOperationException(
                    "Image URL is required.");
            }

            // Validasi ID gambar tidak boleh duplikat.
            var imageIds = command.Images
                .Where(x => x.Id.HasValue)
                .Select(x => x.Id!.Value)
                .ToList();

            if (imageIds.Distinct().Count() != imageIds.Count)
            {
                throw new InvalidOperationException(
                    "Duplicate image ID found.");
            }

            // Pastikan semua gambar memang milik product ini.
            foreach (var imageId in imageIds)
            {
                var imageExists = product.Images
                    .Any(x => x.Id == imageId);

                if (!imageExists)
                {
                    throw new InvalidOperationException(
                        $"Image '{imageId}' does not belong to this product.");
                }
            }

            // Hitung kondisi akhir gambar utama.
            var retainedPrimaryImages = product.Images
                .Count(existing =>
                    !imageIds.Contains(existing.Id) &&
                    existing.IsPrimary);

            var requestedPrimaryImages = command.Images
                .Count(x => x.IsPrimary);

            if (retainedPrimaryImages + requestedPrimaryImages > 1)
            {
                throw new InvalidOperationException(
                    "Only one primary image is allowed.");
            }
        }

        // Update variant yang sudah ada atau tambahkan variant baru.
        foreach (var variantCommand in command.Variants)
        {
            if (variantCommand.Id.HasValue)
            {
                var existingVariant = product.Variants
                    .First(x => x.Id == variantCommand.Id.Value);

                existingVariant.SKU = variantCommand.SKU.Trim();
                existingVariant.Size = variantCommand.Size.Trim();
                existingVariant.IsActive = variantCommand.IsActive;
            }
            else
            {
                var newVariant = new ProductVariant
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    SKU = variantCommand.SKU.Trim(),
                    Size = variantCommand.Size.Trim(),
                    Stock = 0,
                    IsActive = variantCommand.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                product.Variants.Add(newVariant);
                newVariantIds.Add(newVariant.Id);
            }
        }

        // Update image yang sudah ada atau tambahkan image baru.
        if (command.Images is not null)
        {
            foreach (var imageCommand in command.Images)
            {
                if (imageCommand.Id.HasValue)
                {
                    var existingImage = product.Images
                        .FirstOrDefault(x => x.Id == imageCommand.Id.Value);

                    if (existingImage is null)
                    {
                        throw new InvalidOperationException(
                            $"Image '{imageCommand.Id}' does not belong to this product.");
                    }

                    existingImage.ImageUrl = imageCommand.ImageUrl.Trim();
                    existingImage.IsPrimary = imageCommand.IsPrimary;
                    existingImage.SortOrder = imageCommand.SortOrder;
                }
                else
                {
                    var newImage = new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        ImageUrl = imageCommand.ImageUrl.Trim(),
                        IsPrimary = imageCommand.IsPrimary,
                        SortOrder = imageCommand.SortOrder,
                        CreatedAt = DateTime.UtcNow
                    };

                    product.Images.Add(newImage);
                }
            }
        }

        // Update informasi utama product.
        product.CollectionId = command.CollectionId;
        product.CategoryId = command.CategoryId;
        product.Name = command.Name.Trim();
        product.Description = command.Description?.Trim();
        product.Color = command.Color.Trim();
        product.Price = command.Price;
        product.IsActive = command.IsActive;


        // Simpan seluruh perubahan.
        await _productRepository.UpdateAsync(
            product,
            newVariantIds,
            cancellationToken);
    }
}