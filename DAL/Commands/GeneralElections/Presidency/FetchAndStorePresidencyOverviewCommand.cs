using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.Presidency;

public record FetchAndStorePresidencyOverviewCommand(Application.Enum.Entity Entity) : IRequest;

public class FetchAndStorePresidencyOverviewCommandHandler(
    IPresidencyClient centralCommissionClient,
    IPresidencyMappingService mappingService,
    IPresidencyRepository repository,
    IElectionYearsService electionYearsService,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStorePresidencyOverviewCommand>
{
    private readonly IPresidencyClient _centralCommissionClient = centralCommissionClient;
    private readonly IPresidencyMappingService _mappingService = mappingService;
    private readonly IPresidencyRepository _repository = repository;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStorePresidencyOverviewCommand request, CancellationToken cancellationToken)
    {
        var entityCode = request.Entity == Application.Enum.Entity.Federation ? 1 : 2;

        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededYears = _repository
            .GetAllOverviews()
            .Where(overview => overview.Entity == request.Entity)
            .Select(overview => overview.ElectionYear) ?? [];

        electionYears = [.. electionYears.Where(ey => !seededYears.Contains(ey))];

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);
            var overviewUri = $"{cycle.ApiBaseUrl}/race1_entitybasicinfo/{cycle.ResultKey}/{entityCode}";

            var presidencyOverview = await _centralCommissionClient
                .GetPresidentialOverviewAsync(overviewUri)
                .ConfigureAwait(false);

            var entity = _mappingService.MapPresidencyOverview(presidencyOverview, electionYear, request.Entity);

            await _repository.StorePresidencyOverviewAsync(entity);
        }
    }
}
