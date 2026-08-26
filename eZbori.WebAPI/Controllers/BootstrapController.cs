using Application.DTOs;
using Application.Enum;
using Application.Repositories;
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
    IElectionCycleRepository electionCycleRepository,
    IServiceScopeFactory scopeFactory,
    ILogger<BootstrapController> logger)
    : BaseEZboriController(mediator)
{
    private readonly IElectionCycleRepository _electionCycleRepository = electionCycleRepository;
    private static readonly SemaphoreSlim _importLock = new(1, 1);

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportRequest request, CancellationToken cancellationToken)
    {
        if (!await _importLock.WaitAsync(0, cancellationToken))
            return Conflict("Uvoz je već u toku. Pokušajte za nekoliko minuta.");

        var cycle = await _electionCycleRepository.GetByYearAndTypeAsync(request.Year, request.ElectionType);
        if (cycle.DataImported)
        {
            _importLock.Release();
            return Conflict("Podaci za ovaj izborni ciklus su već uvezeni.");
        }

        var electionType = request.ElectionType;
        var year = request.Year;

        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();
            var med = scope.ServiceProvider.GetRequiredService<IMediator>();
            var repo = scope.ServiceProvider.GetRequiredService<IElectionCycleRepository>();
            try
            {
                logger.LogInformation("Bootstrap import started: {Year}/{Type}", year, electionType);

                if (electionType == ElectionType.GeneralElection)
                {
                    await med.Send(new FetchAndStorePresidencyOverviewCommand(Entity.Federation), CancellationToken.None);
                    await med.Send(new FetchAndStorePresidencyOverviewCommand(Entity.RS), CancellationToken.None);
                    await med.Send(new FetchAndStorePresidencyOverviewMunicipalLevelCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStorePresidencyResultsMunicipalLevelCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStorePresidencyResultsCommand(Constituency.Bosniak), CancellationToken.None);
                    await med.Send(new FetchAndStorePresidencyResultsCommand(Constituency.Croat), CancellationToken.None);
                    await med.Send(new FetchAndStorePresidencyResultsCommand(Constituency.Serb), CancellationToken.None);
                    await med.Send(new FetchAndStoreElectoralUnitOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreElectoralUnitPartiesCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreStateMunicipalOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreStateMunicipalPartiesCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityElectoralUnitOverviewCommand(Entity.Federation), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityElectoralUnitOverviewCommand(Entity.RS), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityElectoralUnitPartiesCommand(Entity.Federation), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityElectoralUnitPartiesCommand(Entity.RS), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityPresidentOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityPresidentMunicipalResultsCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreMunicipalOverviewCommand(Entity.Federation), CancellationToken.None);
                    await med.Send(new FetchAndStoreMunicipalOverviewCommand(Entity.RS), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityMunicipalPartyCommand(Entity.Federation), CancellationToken.None);
                    await med.Send(new FetchAndStoreEntityMunicipalPartyCommand(Entity.RS), CancellationToken.None);
                    await med.Send(new FetchAndStoreCantonElectoralUnitOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreCantonElectoralUnitPartyCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreCantonMunicipalOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreCantonMunicipalPartyCommand(), CancellationToken.None);
                }
                else
                {
                    await med.Send(new FetchAndStoreMunicipalCandidateDetailsCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreMunicipalCandidateOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreMunicipalCouncilOverviewCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreMunicipalCouncilPartyCommand(), CancellationToken.None);
                    await med.Send(new FetchAndStoreMunicipalityCouncilMinorityCommand(), CancellationToken.None);
                }

                await repo.MarkImportedAsync(year, electionType);
                logger.LogInformation("Bootstrap import completed successfully: {Year}/{Type}", year, electionType);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bootstrap import failed: {Year}/{Type}", year, electionType);
            }
            finally
            {
                _importLock.Release();
            }
        });

        return Accepted(new { message = "Uvoz je pokrenut u pozadini. Provjeri dropdown za nekoliko minuta." });
    }



    [HttpGet("presidency/federation")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistPresidencyFederationOverview()
    {
        await _mediator.Send(new FetchAndStorePresidencyOverviewCommand(Entity.Federation));

        return Ok();
    }

    [HttpGet("presidency/rs")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistPresidencyRSOverview()
    {
        await _mediator.Send(new FetchAndStorePresidencyOverviewCommand(Entity.RS));

        return Ok();
    }

    [HttpGet("presidency/municipal/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistPresidencyMunicipalOverview()
    {
        await _mediator.Send(new FetchAndStorePresidencyOverviewMunicipalLevelCommand());

        return Ok();
    }

    [HttpGet("presidency/municipal/results")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistPresidencyResults()
    {
        await _mediator.Send(new FetchAndStorePresidencyResultsMunicipalLevelCommand());

        return Ok();
    }

    [HttpGet("presidency")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistPresidencyCroat([FromQuery] Constituency constituency)
    {
        await _mediator.Send(new FetchAndStorePresidencyResultsCommand(constituency));

        return Ok();
    }

    [HttpGet("state/electoralUnits/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistStateElectoralUnitOverview()
    {
        await _mediator.Send(new FetchAndStoreElectoralUnitOverviewCommand());

        return Ok();
    }

    [HttpGet("state/electoralUnits/parties")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistStateElectoralUnitParties()
    {
        await _mediator.Send(new FetchAndStoreElectoralUnitPartiesCommand());

        return Ok();
    }

    [HttpGet("state/municipalities/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistStateMunicipalitiesOverview()
    {
        await _mediator.Send(new FetchAndStoreStateMunicipalOverviewCommand());

        return Ok();
    }

    [HttpGet("state/municipalities/parties")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistStateMunicipalityParties()
    {
        await _mediator.Send(new FetchAndStoreStateMunicipalPartiesCommand());

        return Ok();
    }

    [HttpGet("entity/electoralUnits/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistEntityElectoralUnitOverview([FromQuery] Entity entity)
    {
        await _mediator.Send(new FetchAndStoreEntityElectoralUnitOverviewCommand(entity));

        return Ok();
    }

    [HttpGet("entity/electoralUnits/parties")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistEntityElectoralUnitParties([FromQuery] Entity entity)
    {
        await _mediator.Send(new FetchAndStoreEntityElectoralUnitPartiesCommand(entity));

        return Ok();
    }

    [HttpGet("entity/entityPresident/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistEntityPresidentOverview()
    {
        await _mediator.Send(new FetchAndStoreEntityPresidentOverviewCommand());

        return Ok();
    }

    [HttpGet("entity/entityPresident/municipal")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistEntityPresidentMunicipal()
    {
        await _mediator.Send(new FetchAndStoreEntityPresidentMunicipalResultsCommand());

        return Ok();
    }

    [HttpGet("entity/municipal/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalOverview([FromQuery] Entity entity)
    {
        await _mediator.Send(new FetchAndStoreMunicipalOverviewCommand(entity));

        return Ok();
    }

    [HttpGet("entity/municipal/party")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalParty([FromQuery] Entity entity)
    {
        await _mediator.Send(new FetchAndStoreEntityMunicipalPartyCommand(entity));

        return Ok();
    }

    [HttpGet("entity/canton/electoralUnit/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistCantonElectoralUnitOverview()
    {
        await _mediator.Send(new FetchAndStoreCantonElectoralUnitOverviewCommand());

        return Ok();
    }

    [HttpGet("entity/canton/electoralUnit/parties")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistCantonElectoralUnitParties()
    {
        await _mediator.Send(new FetchAndStoreCantonElectoralUnitPartyCommand());

        return Ok();
    }

    [HttpGet("entity/canton/municipal/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistCantonMunicipalOverview()
    {
        await _mediator.Send(new FetchAndStoreCantonMunicipalOverviewCommand());

        return Ok();
    }

    [HttpGet("entity/canton/municipal/parties")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistCantonMunicipalParties()
    {
        await _mediator.Send(new FetchAndStoreCantonMunicipalPartyCommand());

        return Ok();
    }

    [HttpGet("municipality/candidate/details")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalityCandidateDetails()
    {
        await _mediator.Send(new FetchAndStoreMunicipalCandidateDetailsCommand());

        return Ok();
    }

    [HttpGet("municipality/candidate/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalityCandidateOverview()
    {
        await _mediator.Send(new FetchAndStoreMunicipalCandidateOverviewCommand());

        return Ok();
    }

    [HttpGet("municipality/council/overview")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalityCouncilOverview()
    {
        await _mediator.Send(new FetchAndStoreMunicipalCouncilOverviewCommand());

        return Ok();
    }

    [HttpGet("municipality/council/parties")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalityCouncilParties()
    {
        await _mediator.Send(new FetchAndStoreMunicipalCouncilPartyCommand());

        return Ok();
    }

    [HttpGet("municipality/council/minorities")]
    public async Task<ActionResult<HttpResponse>> FetchAndPersistMunicipalityCouncilMinorities()
    {
        await _mediator.Send(new FetchAndStoreMunicipalityCouncilMinorityCommand());

        return Ok();
    }
}