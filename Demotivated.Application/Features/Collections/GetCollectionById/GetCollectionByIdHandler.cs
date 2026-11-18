using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Collections.GetCollectionById;

public class GetCollectionByIdHandler
{
    private readonly ICollectionRepository _collectionRepository;

    public GetCollectionByIdHandler(
        ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<Collection?> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _collectionRepository.GetByIdAsync(
            id,
            cancellationToken);
    }
}