using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityElectoralUnitPartiesCommand(Application.Enum.Entity Entity) : IRequest;

public class FetchAndStoreEntityElectoralUnitPartiesCommandHandler(
    IEntityClient entityClient,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityRepository repository,
    IEntityMappingService mappingService,
    IElectionYearsService electionYearsService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityElectoralUnitPartiesCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityRepository _repository = repository;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityElectoralUnitPartiesCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitparentpartyresult"
            : "race6_electoralunitparentpartyresult";

        var electionYears = _electionYearsService.GetGeneralElectionYears();
        var disticntEntityElectoralUnits = _municipalityRepo.GetDistinctEntityParliamentElectoralUnits(request.Entity);

        var seededElectoralUnitYears = await _repository.GetElectoralUnitPartiesElectionYearsAsync(disticntEntityElectoralUnits);

        electionYears = electionYears
            .Where(ey => !seededElectoralUnitYears.Contains(ey))
            .ToArray();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var entityElectoralUnit in disticntEntityElectoralUnits)
            {
                var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{entityElectoralUnit}/1";

                var entityElectoralUnitParties = await _entityClient.GetEntityElectoralUnitPartiesAsync(url);

                var models = _mappingService.MapEntityElectoralUnitParties(entityElectoralUnitParties, electionYear, entityElectoralUnit);

                await _repository.StoreElectoralUnitPartiesAsync(models);
            }
        }
    }
}
