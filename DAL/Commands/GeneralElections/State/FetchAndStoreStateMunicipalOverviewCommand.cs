using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreStateMunicipalOverviewCommand(short Year) : IRequest;

public class FetchAndStoreStateMunicipalOverviewCommandHandler(
    IStateClient centralCommisionClient,
    IStateMappingService mappingService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreStateMunicipalOverviewCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommisionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreStateMunicipalOverviewCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteStateMunicipalOverviewAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

            var stateMunicipalOverview = await _centralCommissionClient
                .GetStateMunicipalOverviewsAsync(uri)
                .ConfigureAwait(false);

            var entity = _mappingService.MapStateMunicipalOverview(stateMunicipalOverview, request.Year, municipalityCode);

            await _repository.StoreStateMunicipalOverviews(entity).ConfigureAwait(false);
        }
    }
}
