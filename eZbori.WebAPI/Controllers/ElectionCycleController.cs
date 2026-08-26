using Application.Models;
using DAL.Commands.ElectionCycles;
using DAL.Queries.ElectionCycles;

namespace eZbori.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
[ApiController]
public class ElectionCycleController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ElectionCycle>>> GetAll(CancellationToken cancellationToken)
    {
        var cycles = await _mediator.Send(new GetElectionCyclesQuery(), cancellationToken);
        return Ok(cycles);
    }

    [HttpPost]
    public async Task<ActionResult<ElectionCycle>> Create(
        [FromBody] ElectionCycle cycle, CancellationToken cancellationToken)
    {
        var created = await _mediator.Send(new CreateElectionCycleCommand(cycle), cancellationToken);
        return CreatedAtAction(nameof(GetAll), created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteElectionCycleCommand(id), cancellationToken);
        return NoContent();
    }
}
