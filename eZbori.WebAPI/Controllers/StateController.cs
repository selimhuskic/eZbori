using DAL.Queries.GeneralElections.State;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StateController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet("stateElectoralUnitOverview/{electionYear}/{stateParliamentElectoralUnit}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetStateElectoralUnitOverviews(int electionYear, int stateParliamentElectoralUnit)
    {
        var stateMunicipalResults = 
            await _mediator.Send(new GetStateElectoralUnitOverviewQuery(electionYear, stateParliamentElectoralUnit));

        return Ok(stateMunicipalResults);
    }

    [HttpGet("stateElectoralUnitParties/{electionYear}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetStateElectoralUnitParties(int electionYear)
    {
        var stateMunicipalResults = await _mediator.Send(new GetStateElectoralUnitPartiesQuery(electionYear));

        return Ok(stateMunicipalResults);
    }

    [HttpGet("stateMunicipalOverview/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetStateMunicipalOverview(int electionYear, int municipalityCode)
    {
        var stateMunicipalOverviewResults = await _mediator.Send(new GetStateMunicipalOverviewQuery(electionYear, municipalityCode));

        return Ok(stateMunicipalOverviewResults);
    }

    [HttpGet("stateMunicipalParties/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetStateMunicipalParty(int electionYear, int municipalityCode)
    {
        var stateMunicipalMunicipalResults = await _mediator.Send(new GetStateMunicipalPartyQuery(electionYear, municipalityCode));

        return Ok(stateMunicipalMunicipalResults);
    }
}
