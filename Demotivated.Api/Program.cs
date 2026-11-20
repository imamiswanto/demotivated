using Demotivated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Demotivated.Application.Abstractions.Repositories;
using Demotivated.Infrastructure.Repositories;
using Demotivated.Application.Features.Collections.CreateCollection;
using Demotivated.Application.Features.Collections.GetCollections;
using Demotivated.Application.Features.Collections.GetCollectionById;
using Demotivated.Application.Features.Collections.UpdateCollection;
using Demotivated.Application.Features.Collections.DeleteCollection;
using Demotivated.Application.Features.Categories.CreateCategory;
using Demotivated.Application.Features.Categories.UpdateCategory;
using Demotivated.Application.Features.Categories.GetCategories;
using Demotivated.Application.Features.Categories.GetCategoryById;
using Demotivated.Application.Features.Categories.DeleteCategory;
using Demotivated.Application.Features.Products.CreateProduct;
using Demotivated.Application.Features.Products.GetProducts;
using Demotivated.Application.Features.Products.GetProductById;
using Demotivated.Application.Features.Products.UpdateProduct;
/*using Demotivated.Application.Features.Products.DeleteProduct;*/
using Demotivated.Application.Features.StockAdjustments.CreateStockAdjustment;
using Demotivated.Application.Features.StockMovements.GetStockMovements;
using Demotivated.Application.Features.StockMovements.GetStockMovementById;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DemotivatedDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

//Collection
builder.Services.AddScoped<ICollectionRepository, CollectionRepository>();
builder.Services.AddScoped<CreateCollectionHandler>();
builder.Services.AddScoped<GetCollectionsHandler>();
builder.Services.AddScoped<DeleteCollectionHandler>();
builder.Services.AddScoped<GetCollectionByIdHandler>();
builder.Services.AddScoped<UpdateCollectionHandler>();

// Category
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CreateCategoryHandler>();
builder.Services.AddScoped<UpdateCategoryHandler>();
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<GetCategoryByIdHandler>();
builder.Services.AddScoped<DeleteCategoryHandler>();

// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddScoped<GetProductByIdHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
/*builder.Services.AddScoped<DeleteProductHandler>();*/
builder.Services.AddScoped<CreateStockAdjustmentHandler>();

//Stock Movement
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
builder.Services.AddScoped<GetStockMovementsHandler>();
builder.Services.AddScoped<GetStockMovementByIdHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Demotivated API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();