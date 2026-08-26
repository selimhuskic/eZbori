using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityMunicipalPartyCommand(Application.Enum.Entity Entity) : IRequest;

public class FetchAndStoreEntityMunicipalPartyCommandHandler(
    IElectionYearsService electionYearsService,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityClient entityClient,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityMunicipalPartyCommand>
{
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IEntityRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityMunicipalPartyCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitpartyresult"
            : "race6_electoralunitpartyresult";

        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(request.Entity);

        var seededMunicipalityPartyYears = await _repository
            .GetElectoralUnitOverviewElectionYearsAsync(municipalityCodes);

        electionYears = [.. electionYears.Where(ey => !seededMunicipalityPartyYears.Contains(ey))];

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var entityElectoralUnit in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{entityElectoralUnit}/1";

                var entityMunicipalParties = await _entityClient.GetEntityMunicipalPartyAsync(url)
                    .ConfigureAwait(false);

                var model = _mappingService.MapEntityMunicipalParties(entityMunicipalParties, electionYear, entityElectoralUnit);

                await _repository.StoreMunicipalPartyResultsAsync(model).ConfigureAwait(false);
            }
        }
    }
}
