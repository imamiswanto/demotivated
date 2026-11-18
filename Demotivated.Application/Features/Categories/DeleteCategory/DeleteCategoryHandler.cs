using Demotivated.Application.Abstractions.Repositories;

namespace Demotivated.Application.Features.Categories.DeleteCategory;

public class DeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdIncludingInactiveAsync(
            id,
            cancellationToken);

        if (category is null || !category.IsActive)
        {
            return false;
        }

        category.IsActive = false;

        await _categoryRepository.UpdateAsync(
            category,
            cancellationToken);

        return true;
    }
}