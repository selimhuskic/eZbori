using Application.Services;
using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreElectoralUnitPartiesCommand(short Year) : IRequest;

public class FetchAndStoreElectoralUnitPartiesCommandHandler(
    IStateClient centralCommissionClient,
    IStateMappingService mappingService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreElectoralUnitPartiesCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommissionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreElectoralUnitPartiesCommand request, CancellationToken cancellationToken)
    {
        var stateParliamentElectoralUnits = _municipalityRepo.GetDistinctStateParliamentElectoralUnits();

        await _repository.DeleteElectoralUnitPartiesAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var electoralUnit in stateParliamentElectoralUnits)
        {
            var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitparentpartyresult/{cycle.ResultKey}/{electoralUnit}/1";

            var stateElectoralUnitsParties = await _centralCommissionClient
                .GetElectoralUnitPartiesAsync(uri)
                .ConfigureAwait(false);

            var mappedResult = _mappingService.MapStateElectoralUnitParties(stateElectoralUnitsParties, request.Year, electoralUnit);

            await _repository.StoreStateElectoralUnitPartiesAsync(mappedResult).ConfigureAwait(false);
        }
    }
}
