using DAL.Queries;

namespace eZbori.Web.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class ElectionsController(IMediator mediator) : BaseEZboriController(mediator)
{
    [HttpGet("electionYears/{electionType}")]
    public async Task<ActionResult<IEnumerable<int>>> GetElectionYears(int electionType)
    {
        var electionYEars = await _mediator.Send(new GetElectionYearsQuery(electionType));

        return Ok(electionYEars);
    }

    [HttpGet("municipalities")]
    public async Task<ActionResult<IEnumerable<MunicipalityReadModel>>> GetMunicipalities()
    {
        var municipalities = await _mediator.Send(new GetMunicipalitiesQuery());

        return Ok(municipalities);
    }

    [HttpGet("municipalities/byUnit/{code:int}")]
    public async Task<ActionResult<IEnumerable<string>>> GetMunicipalitiesByUnit(
        int code, CancellationToken ct)
    {
        var municipalities = await _mediator.Send(new GetMunicipalitiesByUnitQuery(code), ct);

        return Ok(municipalities);
    }
}
