using Demotivated.Domain.Entities;

namespace Demotivated.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task AddAsync(
        Product product,
        CancellationToken cancellationToken);

    Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Product?> GetByIdIncludingInactiveAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Product product,
        HashSet<Guid> newVariantIds,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Product product,
        CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(
        string sku,
        CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(
    string sku,
    Guid? excludeProductId,
    CancellationToken cancellationToken);

    Task SaveChangesAsync(
    CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(
    string name,
    Guid excludeProductId,
    CancellationToken cancellationToken);
}