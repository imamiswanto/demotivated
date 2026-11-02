using Demotivated.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demotivated.Infrastructure.Data;

public class DemotivatedDbContext : DbContext
{
    public DemotivatedDbContext(
        DbContextOptions<DemotivatedDbContext> options)
        : base(options)
    {
    }

    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DemotivatedDbContext).Assembly);
    }
}