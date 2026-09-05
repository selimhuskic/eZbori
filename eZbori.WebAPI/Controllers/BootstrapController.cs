using Application.DTOs;
using Application.Enum;
using Application.Repositories;
using Application.Services;
using DAL.Commands.GeneralElections.Canton;
using DAL.Commands.GeneralElections.Entity;
using DAL.Commands.GeneralElections.Presidency;
using DAL.Commands.GeneralElections.State;
using DAL.Commands.LocalElections;

namespace eZbori.Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
[ApiController]
public class BootstrapController(
    IMediator mediator,
    IImportJobRepository importJobRepository,
    IImportQueue importQueue)
    : BaseEZboriController(mediator)
{
    private readonly IImportJobRepository _importJobRepository = importJobRepository;
    private readonly IImportQueue _importQueue = importQueue;

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportRequest request, CancellationToken cancellationToken)
    {
        var job = await _importJobRepository.CreateAsync((int)request.ElectionType, request.Year);
        await _importQueue.PublishAsync(new ImportJobMessage(job.Id, (int)request.ElectionType, request.Year));
        return Accepted(new { jobId = job.Id });
    }

    [HttpGet("import/status/{jobId:guid}")]
    public async Task<IActionResult> GetImportStatus(Guid jobId)
    {
        var job = await _importJobRepository.GetByIdAsync(jobId);
        if (job is null) return NotFound();
        return Ok(new { status = job.Status.ToString(), errorMessage = job.ErrorMessage });
    }

    [HttpGet("presidency/federation")]
    public async Task<IActionResult> FetchAndPersistPresidencyFederationOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStorePresidencyOverviewCommand(Entity.Federation, year));
        return Ok();
    }

    [HttpGet("presidency/rs")]
    public async Task<IActionResult> FetchAndPersistPresidencyRSOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStorePresidencyOverviewCommand(Entity.RS, year));
        return Ok();
    }

    [HttpGet("presidency/municipal/overview")]
    public async Task<IActionResult> FetchAndPersistPresidencyMunicipalOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStorePresidencyOverviewMunicipalLevelCommand(year));
        return Ok();
    }

    [HttpGet("presidency/municipal/results")]
    public async Task<IActionResult> FetchAndPersistPresidencyResults([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStorePresidencyResultsMunicipalLevelCommand(year));
        return Ok();
    }

    [HttpGet("presidency")]
    public async Task<IActionResult> FetchAndPersistPresidencyByConstituency([FromQuery] Constituency constituency, [FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStorePresidencyResultsCommand(constituency, year));
        return Ok();
    }

    [HttpGet("state/electoralUnits/overview")]
    public async Task<IActionResult> FetchAndPersistStateElectoralUnitOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreElectoralUnitOverviewCommand(year));
        return Ok();
    }

    [HttpGet("state/electoralUnits/parties")]
    public async Task<IActionResult> FetchAndPersistStateElectoralUnitParties([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreElectoralUnitPartiesCommand(year));
        return Ok();
    }

    [HttpGet("state/municipalities/overview")]
    public async Task<IActionResult> FetchAndPersistStateMunicipalitiesOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreStateMunicipalOverviewCommand(year));
        return Ok();
    }

    [HttpGet("state/municipalities/parties")]
    public async Task<IActionResult> FetchAndPersistStateMunicipalityParties([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreStateMunicipalPartiesCommand(year));
        return Ok();
    }

    [HttpGet("entity/electoralUnits/overview")]
    public async Task<IActionResult> FetchAndPersistEntityElectoralUnitOverview([FromQuery] Entity entity, [FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreEntityElectoralUnitOverviewCommand(entity, year));
        return Ok();
    }

    [HttpGet("entity/electoralUnits/parties")]
    public async Task<IActionResult> FetchAndPersistEntityElectoralUnitParties([FromQuery] Entity entity, [FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreEntityElectoralUnitPartiesCommand(entity, year));
        return Ok();
    }

    [HttpGet("entity/entityPresident/overview")]
    public async Task<IActionResult> FetchAndPersistEntityPresidentOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreEntityPresidentOverviewCommand(year));
        return Ok();
    }

    [HttpGet("entity/entityPresident/municipal")]
    public async Task<IActionResult> FetchAndPersistEntityPresidentMunicipal([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreEntityPresidentMunicipalResultsCommand(year));
        return Ok();
    }

    [HttpGet("entity/municipal/overview")]
    public async Task<IActionResult> FetchAndPersistMunicipalOverview([FromQuery] Entity entity, [FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreMunicipalOverviewCommand(entity, year));
        return Ok();
    }

    [HttpGet("entity/municipal/party")]
    public async Task<IActionResult> FetchAndPersistMunicipalParty([FromQuery] Entity entity, [FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreEntityMunicipalPartyCommand(entity, year));
        return Ok();
    }

    [HttpGet("entity/canton/electoralUnit/overview")]
    public async Task<IActionResult> FetchAndPersistCantonElectoralUnitOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreCantonElectoralUnitOverviewCommand(year));
        return Ok();
    }

    [HttpGet("entity/canton/electoralUnit/parties")]
    public async Task<IActionResult> FetchAndPersistCantonElectoralUnitParties([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreCantonElectoralUnitPartyCommand(year));
        return Ok();
    }

    [HttpGet("entity/canton/municipal/overview")]
    public async Task<IActionResult> FetchAndPersistCantonMunicipalOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreCantonMunicipalOverviewCommand(year));
        return Ok();
    }

    [HttpGet("entity/canton/municipal/parties")]
    public async Task<IActionResult> FetchAndPersistCantonMunicipalParties([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreCantonMunicipalPartyCommand(year));
        return Ok();
    }

    [HttpGet("municipality/candidate/details")]
    public async Task<IActionResult> FetchAndPersistMunicipalityCandidateDetails([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreMunicipalCandidateDetailsCommand(year));
        return Ok();
    }

    [HttpGet("municipality/candidate/overview")]
    public async Task<IActionResult> FetchAndPersistMunicipalityCandidateOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreMunicipalCandidateOverviewCommand(year));
        return Ok();
    }

    [HttpGet("municipality/council/overview")]
    public async Task<IActionResult> FetchAndPersistMunicipalityCouncilOverview([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreMunicipalCouncilOverviewCommand(year));
        return Ok();
    }

    [HttpGet("municipality/council/parties")]
    public async Task<IActionResult> FetchAndPersistMunicipalityCouncilParties([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreMunicipalCouncilPartyCommand(year));
        return Ok();
    }

    [HttpGet("municipality/council/minorities")]
    public async Task<IActionResult> FetchAndPersistMunicipalityCouncilMinorities([FromQuery] short year)
    {
        await _mediator.Send(new FetchAndStoreMunicipalityCouncilMinorityCommand(year));
        return Ok();
    }
}
