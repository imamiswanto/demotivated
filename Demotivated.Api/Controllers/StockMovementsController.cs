using Demotivated.Application.Features.StockMovements.GetStockMovements;
using Microsoft.AspNetCore.Mvc;

namespace Demotivated.Api.Controllers;

[ApiController]
[Route("api/stock-movements")]
public class StockMovementsController : ControllerBase
{
    private readonly GetStockMovementsHandler _handler;

    public StockMovementsController(
        GetStockMovementsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetStockMovementsQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _handler.Handle(
                query,
                cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}