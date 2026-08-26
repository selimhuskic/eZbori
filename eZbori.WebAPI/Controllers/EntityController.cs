using DAL.Queries.GeneralElections.Entity;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EntityController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet("entityElectoralUnitOverview/{electionYear}/{entityParliamentElectoralUnit}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetEntityElectoralUnitOverviews(int electionYear, int entityParliamentElectoralUnit)
    {
        var entityElectoralUnitOverviews =
            await _mediator.Send(new GetEntityElectoralUnitOverviewQuery(electionYear, entityParliamentElectoralUnit));

        return Ok(entityElectoralUnitOverviews);
    }

    [HttpGet("entityElectoralUnitParty/{electionYear}/{entityParliamentElectoralUnit}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetEntityElectoralUnitParties(int electionYear, int entityParliamentElectoralUnit)
    {
        var entityElectoralUnitParties= await _mediator.Send(new GetEntityElectoralUnitPartiesQuery(electionYear, entityParliamentElectoralUnit));

        return Ok(entityElectoralUnitParties);
    }

    [HttpGet("entityMunicipalOverview/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetEntityMunicipalOverview(int electionYear, int municipalityCode)
    {
        var entityMunicipalOverviewQuery = await _mediator.Send(new GetEntityMunicipalOverviewQuery(electionYear, municipalityCode));

        return Ok(entityMunicipalOverviewQuery);
    }

    [HttpGet("entityMunicipalParties/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetEntityMunicipalParties(int electionYear, int municipalityCode)
    {
        var entityMunicipalParties = await _mediator.Send(new GetEntityMunicipalPartiesQuery(electionYear, municipalityCode));

        return Ok(entityMunicipalParties);
    }

    [HttpGet("entityPresidentMunicipal/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetEntityPresidentMunicipal(int electionYear, int municipalityCode)
    {
        var entityPresidentMunicipal = await _mediator.Send(new GetEntityPresidentMunicipalQuery(electionYear, municipalityCode));

        return Ok(entityPresidentMunicipal);
    }

    [HttpGet("entityPresidentOverview/{electionYear}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetEntityPresidentOverview(int electionYear)
    {
        var entityPresidentOverviews = await _mediator.Send(new GetEntityPresidentOverviewQuery(electionYear));

        return Ok(entityPresidentOverviews);
    }
}
