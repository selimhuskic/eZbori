using DAL.Queries.GeneralElections.Presidency;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PresidencyController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet("overview/{electionYear}/{entity}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetPresidencyOverviews(int electionYear, int entity)
    {
        var presidencyOverview = await _mediator.Send(new GetPresidencyOverviewQuery(electionYear, (Application.Enum.Entity)entity));

        return Ok(presidencyOverview);
    }

    [HttpGet("candidates/{electionYear}/{constituency}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetPresidencyResults(int electionYear, int constituency)
    {
        var presidencyResults =
            await _mediator.Send(new GetPresidencyResultsQuery(electionYear, (Application.Enum.Constituency)constituency));

        return Ok(presidencyResults);
    }

    [HttpGet("municipalOverview/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetPresidencyMunicipalOverview(int electionYear, int municipalityCode)
    {
        var presidencyMunicipalOverviewReadModel =
            await _mediator.Send(new GetPresidencyMunicipalOverviewQuery(electionYear, municipalityCode));

        return Ok(presidencyMunicipalOverviewReadModel);
    }

    [HttpGet("presidencyMunicipalResults/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetPresidencyMunicipalResults(int electionYear, int municipalityCode)
    {
        var presidencyMunicipalResults = 
            await _mediator.Send(new GetPresidencyMunicipalResultsQuery(electionYear, municipalityCode));

        return Ok(presidencyMunicipalResults);
    }
}
