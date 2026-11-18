using Demotivated.Application.Abstractions.Repositories;

namespace Demotivated.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryHandler(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdIncludingInactiveAsync(
            command.Id,
            cancellationToken);

        if (category is null)
        {
            return false;
        }

        var exists = await _categoryRepository.ExistsByNameAsync(
            command.Name,
            cancellationToken);

        if (exists && category.Name != command.Name)
        {
            throw new InvalidOperationException(
                "Category name already exists.");
        }

        category.Name = command.Name;
        category.Description = command.Description;
        category.IsActive = command.IsActive;

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        return true;
    }
}