using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Domain.Entities;
using Demotivated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Demotivated.Infrastructure.Repositories;

public class CollectionRepository : ICollectionRepository
{
    private readonly DemotivatedDbContext _context;

    public CollectionRepository(DemotivatedDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken)
        {
            return await _context.Collections
                .AnyAsync(x => x.Name == name, cancellationToken);
        }

    public async Task AddAsync(
        Collection collection,
        CancellationToken cancellationToken)
    {
        var activeCategoryIds = await _context.Categories
            .Where(x => x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var categoryId in activeCategoryIds)
        {
            collection.CollectionCategories.Add(
                new CollectionCategory
                {
                    CollectionId = collection.Id,
                    CategoryId = categoryId
                });
        }

        await _context.Collections.AddAsync(
            collection,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Collection>> GetAllAsync(
        CancellationToken cancellationToken)
        {
            return await _context.Collections
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
        }

    public async Task<Collection?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
        {
            return await _context.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        }

    public async Task<Collection?> GetByIdIncludingInactiveAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Collections
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }


    public async Task UpdateAsync(
        Collection collection,
        CancellationToken cancellationToken)
    {
        if (collection.IsActive)
        {
            var activeCategoryIds = await _context.Categories
                .Where(x => x.IsActive)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var existingCategoryIds = await _context.CollectionCategories
                .Where(x => x.CollectionId == collection.Id)
                .Select(x => x.CategoryId)
                .ToListAsync(cancellationToken);

            var missingCategoryIds = activeCategoryIds
                .Except(existingCategoryIds);

            var newRelations = missingCategoryIds.Select(categoryId =>
                new CollectionCategory
                {
                    CollectionId = collection.Id,
                    CategoryId = categoryId
                });

            await _context.CollectionCategories.AddRangeAsync(
                newRelations,
                cancellationToken);
        }

        _context.Collections.Update(collection);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Collection collection,
        CancellationToken cancellationToken)
        {
            _context.Collections.Update(collection);
            await _context.SaveChangesAsync(cancellationToken);
        }
}