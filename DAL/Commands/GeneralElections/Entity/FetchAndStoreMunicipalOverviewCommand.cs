using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreMunicipalOverviewCommand(Application.Enum.Entity Entity) : IRequest;

public class FetchAndStoreMunicipalOverviewCommandHandler(
    IEntityClient entityClient,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityRepository repository,
    IEntityMappingService mappingService,
    IElectionYearsService electionYearsService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalOverviewCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityRepository _repository = repository;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalOverviewCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitbasicinfo"
            : "race6_electoralunitbasicinfo";

        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(request.Entity);

        var seededMunicipalityOverviewYears = await _repository
           .GetMunicipalityOverviewYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededMunicipalityOverviewYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{municipalityCode}";

                var entityPresidentOverview = await _entityClient.GetEntityMunicipalOverviewAsync(url)
                    .ConfigureAwait(false);

                var model = _mappingService.MapEntityMunicipalOverview(entityPresidentOverview, electionYear, municipalityCode);

                await _repository.StoreEntityMunicipalOverviewAsync(model).ConfigureAwait(false);
            }
        }
    }
}
