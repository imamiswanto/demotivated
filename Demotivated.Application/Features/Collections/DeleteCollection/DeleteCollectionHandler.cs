using Demotivated.Application.Abstractions.Repositories;

namespace Demotivated.Application.Features.Collections.DeleteCollection;

public class DeleteCollectionHandler
{
    private readonly ICollectionRepository _collectionRepository;

    public DeleteCollectionHandler(
        ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<bool> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (collection is null || !collection.IsActive)
        {
            return false;
        }

        collection.IsActive = false;

        await _collectionRepository.UpdateAsync(
            collection,
            cancellationToken);

        return true;
    }
}