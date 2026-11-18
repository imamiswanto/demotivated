using Demotivated.Application.Abstractions.Repositories;

namespace Demotivated.Application.Features.Collections.UpdateCollection;

public class UpdateCollectionHandler
{
    private readonly ICollectionRepository _collectionRepository;

    public UpdateCollectionHandler(
        ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<bool> Handle(
    UpdateCollectionCommand command,
    CancellationToken cancellationToken)
    {
        var collection = await _collectionRepository.GetByIdIncludingInactiveAsync(
            command.Id,
            cancellationToken);

        if (collection is null)
        {
            return false;
        }

        var exists = await _collectionRepository.ExistsByNameAsync(
            command.Name,
            cancellationToken);

        if (exists && collection.Name != command.Name)
        {
            throw new InvalidOperationException(
                "Collection name already exists.");
        }

        collection.Name = command.Name;
        collection.Description = command.Description;
        collection.IsActive = command.IsActive;

        await _collectionRepository.UpdateAsync(
            collection,
            cancellationToken);

        return true;
    }
}