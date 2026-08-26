using DAL.Queries.GeneralElections.Canton;

namespace eZbori.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CantonController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet("cantonElectoralUnitOverview/{electionYear}/{cantonParliamentElectoralUnit}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetCantonElectoralUnitOverviews(int electionYear, int cantonParliamentElectoralUnit)
    {
        var cantonElectoralUnitOverviews = 
            await _mediator.Send(new GetCantonElectoralUnitOverviewQuery(electionYear, cantonParliamentElectoralUnit));

        return Ok(cantonElectoralUnitOverviews);
    }

    [HttpGet("cantonElectoralUnitParty/{electionYear}/{cantonParliamentElectoralUnit}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetCantonElectoralUnitParties(int electionYear, int cantonParliamentElectoralUnit)
    {
        var cantonElectoralUnitParties = 
            await _mediator.Send(new GetCantonElectoralUnitPartiesQuery(electionYear, cantonParliamentElectoralUnit));

        return Ok(cantonElectoralUnitParties);
    }

    [HttpGet("cantonMunicipalOverview/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableOverviewReadModel>> GetCantonMunicipalOverview(int electionYear, int municipalityCode)
    {
        var cantonMunicipalOverviewQuery = 
            await _mediator.Send(new GetCantonMunicipalOverviewQuery(electionYear, municipalityCode));

        return Ok(cantonMunicipalOverviewQuery);
    }

    [HttpGet("cantonMunicipalParties/{electionYear}/{municipalityCode}")]
    public async Task<ActionResult<TableCandidateReadModel>> GetCantonMunicipalParties(int electionYear, int municipalityCode)
    {
        var cantonMunicipalParties = await _mediator.Send(new GetCantonMunicipalPartiesQuery(electionYear, municipalityCode));

        return Ok(cantonMunicipalParties);
    }
}
