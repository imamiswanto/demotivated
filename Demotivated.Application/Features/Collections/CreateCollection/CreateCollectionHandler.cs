using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Collections.CreateCollection;

public class CreateCollectionHandler
{
    private readonly ICollectionRepository _collectionRepository;

    public CreateCollectionHandler(
        ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<Guid> Handle(
        CreateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var exists = await _collectionRepository.ExistsByNameAsync(
            command.Name,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Collection name already exists.");
        }

        var collection = new Collection
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _collectionRepository.AddAsync(
            collection,
            cancellationToken);

        return collection.Id;
    }
}