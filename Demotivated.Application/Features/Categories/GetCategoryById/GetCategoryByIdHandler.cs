using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;

namespace Demotivated.Application.Features.Categories.GetCategoryById;

public class GetCategoryByIdHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdHandler(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category?> Handle(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetByIdAsync(
            id,
            cancellationToken);
    }
}