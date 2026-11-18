namespace Demotivated.Application.Features.Categories.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description
);