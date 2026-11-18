using Demotivated.Application.Features.Products.CreateProduct;
using Demotivated.Application.Features.Products.GetProducts;
using Demotivated.Application.Features.Products.GetProductById;
using Demotivated.Application.Features.Products.UpdateProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demotivated.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly CreateProductHandler _createProductHandler;
    private readonly GetProductsHandler _getProductsHandler;
    private readonly GetProductByIdHandler _getProductByIdHandler;
    private readonly UpdateProductHandler _updateProductHandler;
    public ProductsController(
        CreateProductHandler createProductHandler,
        GetProductsHandler getProductsHandler,
        GetProductByIdHandler getProductByIdHandler,
        UpdateProductHandler updateProductHandler)
    {
        _createProductHandler = createProductHandler;
        _getProductsHandler = getProductsHandler;
        _getProductByIdHandler = getProductByIdHandler;
        _updateProductHandler = updateProductHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var productId = await _createProductHandler.Handle(
                command,
                cancellationToken);

            return CreatedAtAction(
                nameof(Create),
                new { id = productId },
                new { id = productId });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _getProductsHandler.Handle(
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _getProductByIdHandler.Handle(
            id,
            cancellationToken);

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            await _updateProductHandler.Handle(
                id,
                command,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}