using System.Text;
using Application.DTOs;
using DAL.Commands.Analysis;
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
        var presidencyOverview =
            await _mediator.Send(new GetAnalysisOverviewQuery(request), cancellationToken);

        return Ok(presidencyOverview);
    }

    [AllowAnonymous]
    [HttpPost("parties")]
    public async Task<ActionResult<BaseResultsOverview>> GetParties(
    [FromBody] AnalysisRequest request, CancellationToken cancellationToken)
    {
        var presidencyOverview =
            await _mediator.Send(new GetAnalysisPartiesQuery(request), cancellationToken);

        return Ok(presidencyOverview);
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
