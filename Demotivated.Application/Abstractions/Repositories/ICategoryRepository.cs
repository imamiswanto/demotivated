using Demotivated.Domain.Entities;

namespace Demotivated.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task AddAsync(
        Category category,
        CancellationToken cancellationToken);

    Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken);

    Task <Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Category?> GetByIdIncludingInactiveAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Category category,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Category category,
        CancellationToken cancellationToken);

    Task <bool> IsLinkedToCollectionAsync(
        Guid categoryId,
        Guid collectionId,
        CancellationToken cancellationToken);
}