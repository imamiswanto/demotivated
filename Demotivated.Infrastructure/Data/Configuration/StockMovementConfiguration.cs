using Demotivated.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demotivated.Infrastructure.Data.Configurations;

public class StockMovementConfiguration
    : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.ReferenceType)
            .HasMaxLength(50);

        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_StockMovements_Quantity_NonZero",
                "\"Quantity\" <> 0");

            table.HasCheckConstraint(
                "CK_StockMovements_StockBefore_NonNegative",
                "\"StockBefore\" >= 0");

            table.HasCheckConstraint(
                "CK_StockMovements_StockAfter_NonNegative",
                "\"StockAfter\" >= 0");

            table.HasCheckConstraint(
                "CK_StockMovements_StockConsistency",
                "\"StockAfter\" = \"StockBefore\" + \"Quantity\"");
        });

        builder.HasOne(x => x.ProductVariant)
            .WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProductVariantId);
    }
}