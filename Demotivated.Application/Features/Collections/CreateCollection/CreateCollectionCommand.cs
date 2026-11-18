namespace Demotivated.Application.Features.Collections.CreateCollection;

public record CreateCollectionCommand(
    string Name,
    string? Description
);