using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Application.Features.Categories.CreateCategory;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Categories.CreateCategory;

public class CreateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var exists = await _categoryRepository.ExistsByNameAsync(
            command.Name,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "Category name already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = command.Name.Trim(),
            Description = command.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        return category.Id;
    }
}