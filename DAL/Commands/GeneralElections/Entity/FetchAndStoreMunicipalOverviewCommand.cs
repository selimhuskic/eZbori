using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreMunicipalOverviewCommand(Application.Enum.Entity Entity, short Year) : IRequest;

public class FetchAndStoreMunicipalOverviewCommandHandler(
    IEntityClient entityClient,
    IMunicipalityServiceRepository municipalityRepo,
    IEntityRepository repository,
    IEntityMappingService mappingService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreMunicipalOverviewCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IEntityRepository _repository = repository;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreMunicipalOverviewCommand request, CancellationToken cancellationToken)
    {
        var raceEndpoint = request.Entity == Application.Enum.Entity.Federation
            ? "race4_electoralunitbasicinfo"
            : "race6_electoralunitbasicinfo";

        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(request.Entity);

        await _repository.DeleteEntityMunicipalOverviewAsync(request.Year, municipalityCodes);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{municipalityCode}";

            var entityPresidentOverview = await _entityClient.GetEntityMunicipalOverviewAsync(url)
                .ConfigureAwait(false);

            var model = _mappingService.MapEntityMunicipalOverview(entityPresidentOverview, request.Year, municipalityCode);

            await _repository.StoreEntityMunicipalOverviewAsync(model).ConfigureAwait(false);
        }
    }
}
