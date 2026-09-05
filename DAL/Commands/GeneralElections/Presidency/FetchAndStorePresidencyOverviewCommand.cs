using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyOverviewCommand(Application.Enum.Entity Entity, short Year) : IRequest;

public class FetchAndStorePresidencyOverviewCommandHandler(
    IPresidencyClient centralCommissionClient,
    IPresidencyMappingService mappingService,
    IPresidencyRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyOverviewCommand>
{
    private readonly IPresidencyClient _centralCommissionClient = centralCommissionClient;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyOverviewCommand request, CancellationToken cancellationToken)
    {
        var entityCode = request.Entity == Application.Enum.Entity.Federation ? 1 : 2;

        await _repository.DeletePresidencyOverviewAsync(request.Year, request.Entity);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);
        var overviewUri = $"{cycle.ApiBaseUrl}/race1_entitybasicinfo/{cycle.ResultKey}/{entityCode}";

        var presidencyOverview = await _centralCommissionClient
            .GetPresidentialOverviewAsync(overviewUri)
            .ConfigureAwait(false);

        var entity = _mappingService.MapPresidencyOverview(presidencyOverview, request.Year, request.Entity);

        await _repository.StorePresidencyOverviewAsync(entity);
    }
}
