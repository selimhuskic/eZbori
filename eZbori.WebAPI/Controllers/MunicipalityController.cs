using DAL.Queries.LocalElections;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MunicipalityController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet("municipalityCandidateDetails/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetMunicipalityCandidateDetails(int electionYear, int municipalityCode)
    {
        var presidencyOverview =
            await _mediator.Send(new GetMunicipalityCandidateDetailsQuery(electionYear, municipalityCode));

        return Ok(presidencyOverview);
    }

    [HttpGet("municipalityCandidateOverview/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetMunicipalityCandidateOverview(int electionYear, int municipalityCode)
    {
        var presidencyResults = 
            await _mediator.Send(new GetMunicipalityCandidateOverviewQuery(electionYear, municipalityCode));

        return Ok(presidencyResults);
    }

    [HttpGet("municipalityCouncilMinority/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetMunicipalityCandidateCouncilMinority(int electionYear, int municipalityCode)
    {
        var presidencyMunicipalOverview = 
            await _mediator.Send(new GetMunicipalityCouncilMinorityQuery(electionYear, municipalityCode));

        return Ok(presidencyMunicipalOverview);
    }

    [HttpGet("municipalityCouncilOverview/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetMunicipalityCouncilOverview(int electionYear, int municipalityCode)
    {
        var presidencyMunicipalResults =
            await _mediator.Send(new GetMunicipalityCouncilOverviewQuery(electionYear, municipalityCode));

        return Ok(presidencyMunicipalResults);
    }

    [HttpGet("municipalityCouncilParty/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetMunicipalityCouncilParty(int electionYear, int municipalityCode)
    {
        var presidencyMunicipalResults = await _mediator.Send(new GetMunicipalityCouncilPartyQuery(electionYear, municipalityCode));

        return Ok(presidencyMunicipalResults);
    }
}
