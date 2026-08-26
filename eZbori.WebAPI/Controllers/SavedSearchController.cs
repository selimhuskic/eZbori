using DAL.Commands.SavedSearch;
using DAL.Queries.SavedSearch;
using System.Security.Claims;

namespace eZbori.Web.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SavedSearchController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application.Models.SavedSearch>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var results = await _mediator.Send(new GetSavedSearchesByUserQuery(userId.Value), cancellationToken);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult<Application.Models.SavedSearch>> Create(
        [FromBody] Application.Models.SavedSearch search, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        search.UserId = userId.Value;
        var created = await _mediator.Send(new CreateSavedSearchCommand(search), cancellationToken);
        return CreatedAtAction(nameof(GetMine), created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        await _mediator.Send(new DeleteSavedSearchCommand(id, userId.Value), cancellationToken);
        return NoContent();
    }

    [HttpDelete("all")]
    public async Task<IActionResult> DeleteAll(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        await _mediator.Send(new DeleteAllSavedSearchesCommand(userId.Value), cancellationToken);
        return NoContent();
    }

    private int? GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : null;
    }
}
