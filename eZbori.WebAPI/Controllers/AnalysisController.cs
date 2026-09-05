using System.Text;
using Application.DTOs;
using DAL.Commands.Analysis;
using DAL.Queries;
using DAL.Queries.Analysis;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalysisController(IMediator mediator) : BaseEZboriController(mediator)
{
    [AllowAnonymous]
    [HttpPost("overview")]
    public async Task<ActionResult<BaseResultsOverview>> GetOverview(
        [FromBody] AnalysisRequest request, CancellationToken cancellationToken)
    {
        var denied = await CheckGuestAccessAsync(request, cancellationToken);
        if (denied is not null) return denied;

        var presidencyOverview =
            await _mediator.Send(new GetAnalysisOverviewQuery(request), cancellationToken);

        return Ok(presidencyOverview);
    }

    [AllowAnonymous]
    [HttpPost("parties")]
    public async Task<ActionResult<BaseResultsOverview>> GetParties(
    [FromBody] AnalysisRequest request, CancellationToken cancellationToken)
    {
        var denied = await CheckGuestAccessAsync(request, cancellationToken);
        if (denied is not null) return denied;

        var presidencyOverview =
            await _mediator.Send(new GetAnalysisPartiesQuery(request), cancellationToken);

        return Ok(presidencyOverview);
    }

    private async Task<ActionResult?> CheckGuestAccessAsync(AnalysisRequest request, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true) return null;

        if (request.ElectionType == Application.Enum.ElectionType.LocalElection)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Lokalni izbori su dostupni samo registrovanim korisnicima."
            });
        }            

        var years = await _mediator.Send(
            new GetElectionYearsQuery((int)Application.Enum.ElectionType.GeneralElection), cancellationToken);
        
        var latestYear = years.DefaultIfEmpty(0).Max();

        if (request.SelectedYear != latestYear)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "Neregistrovani korisnici mogu pregledati samo najnoviju godinu opštih izbora."
            });
        }            

        return null;
    }

    [Authorize]
    [HttpPost("export/csv")]
    public async Task<IActionResult> ExportCsv(
        [FromBody] AnalysisRequest request, CancellationToken cancellationToken)
    {
        var csv = await _mediator.Send(new ExportAnalysisCsvCommand(request), cancellationToken);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv; charset=utf-8", "ezbori_export.csv");
    }
}
