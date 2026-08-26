using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreElectoralUnitPartiesCommand() : IRequest;

public class FetchAndStoreElectoralUnitPartiesCommandHandler(
    IStateClient centralCommissionClient,
    IStateMappingService mappingService,
    IElectionYearsService electionYearsService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreElectoralUnitPartiesCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommissionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IElectionYearsService _electionYearsService = electionYearsService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreElectoralUnitPartiesCommand request, CancellationToken cancellationToken)
    {
        var electionYears = _electionYearsService.GetGeneralElectionYears();

        var seededElectoralUnitYears = await _repository
            .GetElectoralUnitPartiesElectionYearsAsync();

        electionYears = electionYears
            .Where(ey => !seededElectoralUnitYears.Contains(ey))
            .ToArray();

        var stateParliamentElectoralUnits = _municipalityRepo.GetDistinctStateParliamentElectoralUnits();

        foreach (var electionYear in electionYears)
        {
            var cycle = await _cycleRepository.GetByYearAndTypeAsync((short)electionYear, ElectionType.GeneralElection);

            foreach (var electoralUnit in stateParliamentElectoralUnits)
            {
                var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitparentpartyresult/{cycle.ResultKey}/{electoralUnit}/1";

                var stateElectoralUnitsParties = await _centralCommissionClient
                    .GetElectoralUnitPartiesAsync(uri)
                    .ConfigureAwait(false);

                var mappedResult = _mappingService.MapStateElectoralUnitParties(stateElectoralUnitsParties, electionYear, electoralUnit);

                await _repository.StoreStateElectoralUnitPartiesAsync(mappedResult).ConfigureAwait(false);
            }
        }
    }
}
