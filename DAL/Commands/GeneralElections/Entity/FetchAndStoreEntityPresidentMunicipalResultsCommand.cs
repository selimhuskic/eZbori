using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Entity;

public record FetchAndStoreEntityPresidentMunicipalResultsCommand(short Year) : IRequest;

public class FetchAndStoreEntityPresidentMunicipalResultsCommandHandler(
    IEntityClient entityClient,
    IEntityMappingService mappingService,
    IEntityRepository repository,
    IMunicipalityServiceRepository municipalityRepo,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreEntityPresidentMunicipalResultsCommand>
{
    private readonly IEntityClient _entityClient = entityClient;
    private readonly IEntityMappingService _mappingService = mappingService;
    private readonly IEntityRepository _repository = repository;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreEntityPresidentMunicipalResultsCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes(Application.Enum.Entity.RS);

        await _repository.DeleteEntityPresidentMunicipalAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var url = $"{cycle.ApiBaseUrl}/race5_electoralunitcandidatesresult/{cycle.ResultKey}/{municipalityCode}/1";

            var entityPresidentMunicipal = await _entityClient.GetEntityPresidentMunicipalCandidateAsync(url)
                .ConfigureAwait(false);

            var model = _mappingService.MapPresidentMunicipal(entityPresidentMunicipal, request.Year, municipalityCode);

            await _repository.StorePresidentMunicipalAsync(model).ConfigureAwait(false);
        }
    }
}
