using Demotivated.Application.Features.Categories.CreateCategory;
using Demotivated.Application.Features.Categories.UpdateCategory;
using Demotivated.Application.Features.Categories.GetCategories;
using Demotivated.Application.Features.Categories.GetCategoryById;
using Demotivated.Application.Features.Categories.DeleteCategory;
using Microsoft.AspNetCore.Mvc;

namespace Demotivated.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _handler;
    private readonly UpdateCategoryHandler _updateHandler;
    private readonly GetCategoriesHandler _getCategoriesHandler;
    private readonly GetCategoryByIdHandler _getCategoryByIdHandler;
    private readonly DeleteCategoryHandler _deleteCategoryHandler;
    public CategoriesController(
        CreateCategoryHandler handler,
        UpdateCategoryHandler updateHandler,
        GetCategoriesHandler getCategoriesHandler,
        GetCategoryByIdHandler getCategoryByIdHandler,
        DeleteCategoryHandler deleteCategoryHandler)
    {
        _handler = handler;
        _updateHandler = updateHandler;
        _getCategoriesHandler = getCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
        _deleteCategoryHandler = deleteCategoryHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = await _handler.Handle(command, cancellationToken);

            return CreatedAtAction(
                nameof(Create),
                new { id },
                new { id });
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
        var categories = await _getCategoriesHandler.Handle(cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _getCategoryByIdHandler.Handle(id, cancellationToken);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _updateHandler.Handle(
                command with { Id = id },
                cancellationToken);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _deleteCategoryHandler.Handle(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}