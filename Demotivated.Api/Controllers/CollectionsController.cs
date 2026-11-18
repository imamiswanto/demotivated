using Demotivated.Application.Features.Collections.CreateCollection;
using Demotivated.Application.Features.Collections.GetCollections;
using Demotivated.Application.Features.Collections.GetCollectionById;
using Demotivated.Application.Features.Collections.UpdateCollection;
using Demotivated.Application.Features.Collections.DeleteCollection;

using Microsoft.AspNetCore.Mvc;


namespace Demotivated.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly CreateCollectionHandler _handler;
    private readonly GetCollectionsHandler _getHandler;
    private readonly GetCollectionByIdHandler _getByIdHandler;
    private readonly UpdateCollectionHandler _updateHandler;
    private readonly DeleteCollectionHandler _deleteHandler;
    public CollectionsController(
        CreateCollectionHandler handler,
        GetCollectionsHandler getHandler,
        GetCollectionByIdHandler getByIdHandler,
        UpdateCollectionHandler updateHandler,
        DeleteCollectionHandler deleteHandler)
        {
            _handler = handler;
            _getHandler = getHandler;
            _getByIdHandler = getByIdHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        CreateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        var collectionId = await _handler.Handle(
            command,
            cancellationToken);

        return Created(
            $"/api/collections/{collectionId}",
            new { id = collectionId });
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCollections(
        CancellationToken cancellationToken)
    {
        var collections = await _getHandler.Handle(
            cancellationToken);

        return Ok(collections);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var collection = await _getByIdHandler.Handle(
            id,
            cancellationToken);

        if (collection is null)
        {
            return NotFound();
        }

        return Ok(collection);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCollectionCommand command,
        CancellationToken cancellationToken)
    {
        command = command with { Id = id };

        var updated = await _updateHandler.Handle(
            command,
            cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _deleteHandler.Handle(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}