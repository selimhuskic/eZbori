using Application.DTOs;
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
    public async Task<ActionResult<IEnumerable<SavedSearchReadModel>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var results = await _mediator.Send(new GetSavedSearchesByUserQuery(userId.Value), cancellationToken);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult<SavedSearchReadModel>> Create(
        [FromBody] CreateSavedSearchRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var search = new Application.Models.SavedSearch
        {
            UserId = userId.Value,
            ElectionType = request.ElectionType,
            ElectionYear = request.ElectionYear,
            AnalysisSubject = request.AnalysisSubject,
            ElectoralUnit = request.ElectoralUnit,
            MunicipalityCode = request.MunicipalityCode,
        };
        var created = await _mediator.Send(new CreateSavedSearchCommand(search), cancellationToken);
        var response = new SavedSearchReadModel(created.Id, created.ElectionType, created.ElectionYear,
            created.AnalysisSubject, created.ElectoralUnit, created.MunicipalityCode, created.CreatedAt);
        return CreatedAtAction(nameof(GetMine), response);
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
