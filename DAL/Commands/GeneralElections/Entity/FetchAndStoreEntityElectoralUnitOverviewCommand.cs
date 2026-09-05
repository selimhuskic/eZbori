using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityElectoralUnitOverviewCommand(Application.Enum.Entity Entity, short Year) : IRequest;

public class FetchAndStoreEntityElectoralUnitOverviewCommandHandler(
    IEntityClient entityClient,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityElectoralUnitOverviewCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityRepository _repository = repository;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityElectoralUnitOverviewCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitparentbasicinfo"
            : "race6_electoralunitparentbasicinfo";

        var distinctEntityElectoralUnits = _municipalityRepo.GetDistinctEntityParliamentElectoralUnits(request.Entity);

        await _repository.DeleteEntityElectoralUnitOverviewAsync(request.Year, distinctEntityElectoralUnits);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var entityElectoralUnit in distinctEntityElectoralUnits)
        {
            var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{entityElectoralUnit}";

            var entityElectoralUnitOverview = await _entityClient.GetEntityElectoralUnitOverviewAsync(url);

            var model = _mappingService.MapEntityElectoralUnitOverview(entityElectoralUnitOverview, request.Year, entityElectoralUnit);

            await _repository.StoreElectoralUnitOverviewAsync(model);
        }
    }
}
