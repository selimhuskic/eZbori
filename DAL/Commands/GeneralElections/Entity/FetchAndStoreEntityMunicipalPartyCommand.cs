using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityMunicipalPartyCommand(Application.Enum.Entity Entity, short Year) : IRequest;

public class FetchAndStoreEntityMunicipalPartyCommandHandler(
    IMunicipalityServiceRepository municipalityRepo,
    IEntityClient entityClient,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityMunicipalPartyCommand>
{
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

        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(request.Entity);

        await _repository.DeleteEntityMunicipalPartyAsync(request.Year, municipalityCodes);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var entityElectoralUnit in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/{raceEndpoint}/{cycle.ResultKey}/{entityElectoralUnit}/1";

            var entityMunicipalParties = await _entityClient.GetEntityMunicipalPartyAsync(url)
                .ConfigureAwait(false);

            var model = _mappingService.MapEntityMunicipalParties(entityMunicipalParties, request.Year, entityElectoralUnit);

            await _repository.StoreMunicipalPartyResultsAsync(model).ConfigureAwait(false);
        }
    }
}
