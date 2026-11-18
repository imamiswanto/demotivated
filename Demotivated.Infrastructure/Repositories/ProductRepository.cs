using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;
using Demotivated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;


namespace Demotivated.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DemotivatedDbContext _context;

    public ProductRepository(DemotivatedDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(
            product,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Include(x => x.Collection)
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Include(x => x.Images)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        Guid excludeProductId,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AnyAsync(
                x => x.Name == name
                    && x.Id != excludeProductId,
                cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(x => x.Collection)
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsActive,
                cancellationToken);
    }

    public async Task<Product?> GetByIdIncludingInactiveAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(x => x.Collection)
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Product product,
        HashSet<Guid> newVariantIds,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // Simpan kondisi akhir SKU setiap variant.
            var originalSkus = product.Variants.ToDictionary(
                x => x.Id,
                x => x.SKU);

            // Simpan kondisi akhir primary image.
            var primaryStates = product.Images.ToDictionary(
                x => x.Id,
                x => x.IsPrimary);

            // Tahap 1: gunakan SKU sementara yang unik.
            foreach (var variant in product.Variants)
            {
                variant.SKU = $"TMP-{Guid.NewGuid():N}";
            }

            foreach (var variantId in newVariantIds)
            {
                var newVariant = product.Variants
                    .First(x => x.Id == variantId);

                _context.Entry(newVariant).State = EntityState.Added;
            }

            // Semua gambar sementara dibuat bukan primary.
            foreach (var image in product.Images)
            {
                image.IsPrimary = false;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Tahap 2: kembalikan SKU sesuai request.
            foreach (var variant in product.Variants)
            {
                variant.SKU = originalSkus[variant.Id];
            }

            // Kembalikan status primary sesuai request.
            foreach (var image in product.Images)
            {
                image.IsPrimary = primaryStates[image.Id];
            }

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        product.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        CancellationToken cancellationToken)
    {
        return await ExistsBySkuAsync(
            sku,
            null,
            cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludeProductId,
        CancellationToken cancellationToken)
    {
        var normalizedSku = sku.Trim().ToLower();

        var query = _context.ProductVariants.AsQueryable();

        if (excludeProductId.HasValue)
        {
            query = query.Where(
                x => x.ProductId != excludeProductId.Value);
        }

        return await query.AnyAsync(
            x => x.SKU.ToLower() == normalizedSku,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}