using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityElectoralUnitOverviewCommand(Application.Enum.Entity Entity) : IRequest;

public class FetchAndStoreEntityElectoralUnitOverviewCommandHandler(
    IEntityClient entityClient,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityMappingService mappingService,
    IElectionYearsService electionYearsService,
    IEntityRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityElectoralUnitOverviewCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityRepository _repository = repository;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityElectoralUnitOverviewCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitparentbasicinfo"
            : "race6_electoralunitparentbasicinfo";

        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var distinctEntityElectoralUnits = _municipalityRepo.GetDistinctEntityParliamentElectoralUnits(request.Entity);

        var seededElectoralUnitYears = await _repository
            .GetElectoralUnitOverviewElectionYearsAsync(distinctEntityElectoralUnits);

        electionYears = [.. electionYears.Where(ey => !seededElectoralUnitYears.Contains(ey))];

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var entityElectoralUnit in distinctEntityElectoralUnits)
            {
                var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{entityElectoralUnit}";

                var entityElectoralUnitOverview = await _entityClient.GetEntityElectoralUnitOverviewAsync(url);

                var model = _mappingService.MapEntityElectoralUnitOverview(entityElectoralUnitOverview, electionYear, entityElectoralUnit);

                await _repository.StoreElectoralUnitOverviewAsync(model);
            }
        }
    }
}
