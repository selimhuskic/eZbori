using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreElectoralUnitOverviewCommand(short Year) : IRequest;

public class FetchAndStoreElectoralUnitOverviewCommandHandler(
    IStateClient centralCommissionClient,
    IStateMappingService mappingService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreElectoralUnitOverviewCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommissionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreElectoralUnitOverviewCommand request, CancellationToken cancellationToken)
    {
        var stateParliamentElectoralUnits = _municipalityRepo.GetDistinctStateParliamentElectoralUnits();

        await _repository.DeleteElectoralUnitOverviewAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var electoralUnit in stateParliamentElectoralUnits)
        {
            var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitparentbasicinfo/{cycle.ResultKey}/{electoralUnit}";

            var stateElectoralUnitsOverview = await _centralCommissionClient
                .GetElectoralUnitOverviewAsync(uri)
                .ConfigureAwait(false);

            var stateElectoralUnitResults = _mappingService.MapStateElectoralUnitOverview(stateElectoralUnitsOverview, request.Year);

            await _repository.StoreElectoralUnitOverviewAsync(stateElectoralUnitResults).ConfigureAwait(false);
        }
    }
}
