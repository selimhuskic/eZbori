using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreElectoralUnitOverviewCommand() : IRequest;

public class FetchAndStoreElectoralUnitOverviewCommandHandler(
    IStateClient centralCommissionClient,
    IStateMappingService mappingService,
    IElectionYearsService electionYearsService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreElectoralUnitOverviewCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommissionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreElectoralUnitOverviewCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededElectoralUnitYears = await _repository
            .GetElectoralUnitOverviewElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededElectoralUnitYears.Contains(ey))
            .ToArray();

        var stateParliamentElectoralUnits = _municipalityRepo.GetDistinctStateParliamentElectoralUnits();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var electoralUnit in stateParliamentElectoralUnits)
            {
                var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitparentbasicinfo/{cycle.ResultKey}/{electoralUnit}";

                var stateElectoralUnitsOverview = await _centralCommissionClient
                    .GetElectoralUnitOverviewAsync(uri)
                    .ConfigureAwait(false);

                var stateElectoralUnitResults = _mappingService.MapStateElectoralUnitOverview(stateElectoralUnitsOverview, electionYear);

                await _repository.StoreElectoralUnitOverviewAsync(stateElectoralUnitResults).ConfigureAwait(false);
            }
        }
    }
}
