using Demotivated.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demotivated.Infrastructure.Data.Configurations;

public class ProductConfiguration
    : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);
        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.ToTable(table => table.HasCheckConstraint(
            "CK_Product_Price_NonNegative",
            "\"Price\" >= 0"));

        builder.HasIndex(x => new { x.CategoryId, x.Name });

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Collection)
            .WithMany()
            .HasForeignKey(x => x.CollectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CollectionCategory>()
            .WithMany()
            .HasForeignKey(x => new
            {
                x.CollectionId,
                x.CategoryId
            })
            .OnDelete(DeleteBehavior.Restrict);
    }
}