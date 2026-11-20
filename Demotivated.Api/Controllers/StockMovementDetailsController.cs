using Demotivated.Application.Features.StockMovements.GetStockMovementById;
using Microsoft.AspNetCore.Mvc;

namespace Demotivated.Api.Controllers;

[ApiController]
[Route("api/stock-movements")]
public class StockMovementDetailsController : ControllerBase
{
    private readonly GetStockMovementByIdHandler _handler;

    public StockMovementDetailsController(
        GetStockMovementByIdHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _handler.Handle(
                id,
                cancellationToken);

            if (result is null)
                return NotFound(new
                {
                    message = "Stock movement not found."
                });

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}