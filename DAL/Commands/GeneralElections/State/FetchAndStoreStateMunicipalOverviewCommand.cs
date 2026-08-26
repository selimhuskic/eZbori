using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreStateMunicipalOverviewCommand() : IRequest;

public class FetchAndStoreStateMunicipalOverviewCommandHandler(
    IStateClient centralCommisionClient,
    IStateMappingService mappingService,
    IElectionYearsService electionYearsService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreStateMunicipalOverviewCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommisionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreStateMunicipalOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededElectoralUnitYears = await _repository
            .GetElectoralUnitMunicipalOverviewElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededElectoralUnitYears.Contains(ey))
            .ToArray();

        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var municipalityCode in municipalityCodes)
            {
                var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitbasicinfo/{cycle.ResultKey}/{municipalityCode}";

                var stateMunicipalOverview = await _centralCommissionClient
                    .GetStateMunicipalOverviewsAsync(uri)
                    .ConfigureAwait(false);

                var entity = _mappingService.MapStateMunicipalOverview(stateMunicipalOverview, electionYear, municipalityCode);

                await _repository.StoreStateMunicipalOverviews(entity).ConfigureAwait(false);
            }
        }
    }
}
