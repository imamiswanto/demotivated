using Demotivated.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demotivated.Infrastructure.Data.Configurations;

public class CollectionCategoryConfiguration
    : IEntityTypeConfiguration<CollectionCategory>
{
    public void Configure(EntityTypeBuilder<CollectionCategory> builder)
    {
        builder.HasKey(x => new
        {
            x.CollectionId,
            x.CategoryId
        });

        builder.HasOne(x => x.Collection)
            .WithMany(x => x.CollectionCategories)
            .HasForeignKey(x => x.CollectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.CollectionCategories)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}