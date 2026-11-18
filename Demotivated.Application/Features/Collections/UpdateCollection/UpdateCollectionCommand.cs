namespace Demotivated.Application.Features.Collections.UpdateCollection;

public record UpdateCollectionCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
);