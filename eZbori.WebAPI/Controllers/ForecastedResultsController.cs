using DAL.Commands.ForecastedResults;
using DAL.Queries.ForecastedResults;

namespace eZbori.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
[ApiController]
public class ForecastedResultsController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ForecastedResult>>> GetAll(CancellationToken cancellationToken)
    {
        var results = await _mediator.Send(new GetAllForecastedResultsQuery(), cancellationToken);
        return Ok(results);
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
