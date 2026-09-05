using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityElectoralUnitPartiesCommand(Application.Enum.Entity Entity, short Year) : IRequest;

public class FetchAndStoreEntityElectoralUnitPartiesCommandHandler(
    IEntityClient entityClient,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityRepository repository,
    IEntityMappingService mappingService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityElectoralUnitPartiesCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityRepository _repository = repository;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityElectoralUnitPartiesCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitparentpartyresult"
            : "race6_electoralunitparentpartyresult";

        var distinctEntityElectoralUnits = _municipalityRepo.GetDistinctEntityParliamentElectoralUnits(request.Entity);

        await _repository.DeleteEntityElectoralUnitPartiesAsync(request.Year, distinctEntityElectoralUnits);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var entityElectoralUnit in distinctEntityElectoralUnits)
        {
            var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{entityElectoralUnit}/1";

            var entityElectoralUnitParties = await _entityClient.GetEntityElectoralUnitPartiesAsync(url);

            var models = _mappingService.MapEntityElectoralUnitParties(entityElectoralUnitParties, request.Year, entityElectoralUnit);

            await _repository.StoreElectoralUnitPartiesAsync(models);
        }
    }
}
