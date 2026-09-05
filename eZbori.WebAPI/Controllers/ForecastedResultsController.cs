using DAL.Commands.ForecastedResults;
using DAL.Queries.ForecastedResults;

namespace eZbori.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
[ApiController]
public class ForecastedResultsController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var (items, total) = await _mediator.Send(new GetAllForecastedResultsQuery(page, pageSize), cancellationToken);
        return Ok(new { items, total, page, pageSize });
    }

    [HttpPost]
    public async Task<ActionResult<ForecastedResult>> Create([FromBody] ForecastedResult result, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateForecastedResultCommand(result), cancellationToken);
        return CreatedAtAction(nameof(GetAll), created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteForecastedResultCommand(id), cancellationToken);
        return NoContent();
    }
}
