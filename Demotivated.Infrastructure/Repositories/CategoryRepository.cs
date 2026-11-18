using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;
using Demotivated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demotivated.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DemotivatedDbContext _context;

    public CategoryRepository(DemotivatedDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        var collectionIds = await _context.Collections
            .Where(x => x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var collectionId in collectionIds)
        {
            category.CollectionCategories.Add(
                new CollectionCategory
                {
                    CollectionId = collectionId,
                    CategoryId = category.Id
                });
        }

        await _context.Categories.AddAsync(
            category,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id && x.IsActive,
                cancellationToken);
    }

    public async Task<Category?> GetByIdIncludingInactiveAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        if (category.IsActive)
        {
            var activeCollectionIds = await _context.Collections
                .Where(x => x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var existingCollectionIds = await _context.CollectionCategories
                .Where(x => x.CategoryId == category.Id)
                .Select(x => x.CollectionId)
                .ToListAsync(cancellationToken);

            var missingCollectionIds = activeCollectionIds
                .Except(existingCollectionIds);

            var newRelations = missingCollectionIds.Select(collectionId =>
                new CollectionCategory
                {
                    CollectionId = collectionId,
                    CategoryId = category.Id
                });

            await _context.CollectionCategories.AddRangeAsync(
                newRelations,
                cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsLinkedToCollectionAsync(
        Guid categoryId,
        Guid collectionId,
        CancellationToken cancellationToken)
    {
        return await _context.CollectionCategories
            .AnyAsync(x => x.CategoryId == categoryId && x.CollectionId == collectionId, cancellationToken);
    }
}