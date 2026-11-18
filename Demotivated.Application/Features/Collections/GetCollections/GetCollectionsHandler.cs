using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Collections.GetCollections;

public class GetCollectionsHandler
{
    private readonly ICollectionRepository _collectionRepository;

    public GetCollectionsHandler(
        ICollectionRepository collectionRepository)
    {
        _collectionRepository = collectionRepository;
    }

    public async Task<List<Collection>> Handle(
        CancellationToken cancellationToken)
    {
        return await _collectionRepository.GetAllAsync(
            cancellationToken);
    }
}