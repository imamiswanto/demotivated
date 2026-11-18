using Demotivated.Domain.Entities;


namespace Demotivated.Application.Abstractions.Repositories;

public interface ICollectionRepository
{
    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken);

    Task AddAsync(
        Collection collection,
        CancellationToken cancellationToken);

    Task<List<Collection>> GetAllAsync(
    CancellationToken cancellationToken);

    Task<Collection?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken);

    Task UpdateAsync(
    Collection collection,
    CancellationToken cancellationToken);

    Task DeleteAsync(
    Collection collection,
    CancellationToken cancellationToken);

    Task<Collection?> GetByIdIncludingInactiveAsync(
    Guid id,
    CancellationToken cancellationToken);
}