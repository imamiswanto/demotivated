using Demotivated.Application.Features.StockAdjustments.CreateStockAdjustment;
using Microsoft.AspNetCore.Mvc;

namespace Demotivated.Api.Controllers;

[ApiController]
[Route("api/stock-adjustments")]
public class StockAdjustmentsController : ControllerBase
{
    private readonly CreateStockAdjustmentHandler _handler;

    public StockAdjustmentsController(
        CreateStockAdjustmentHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateStockAdjustmentCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _handler.Handle(
                command,
                cancellationToken);

            return Ok(result);
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