using Application.Services;

using Application.Services;
using External.CentralElectionCommiteeHttpClients;
using MediatR;

namespace DAL.Commands.GeneralElections.State;

public record FetchAndStoreStateMunicipalPartiesCommand(short Year) : IRequest;

public class FetchAndStoreStateMunicipalPartiesCommandHandler(
    IStateClient centralCommissionClient,
    IStateMappingService mappingService,
    IMunicipalityServiceRepository municipalityRepo,
    IStateRepository repository,
    IElectionCycleRepository cycleRepository) : IRequestHandler<FetchAndStoreStateMunicipalPartiesCommand>
{
    private readonly IStateClient _centralCommissionClient = centralCommissionClient;
    private readonly IStateMappingService _mappingService = mappingService;
    private readonly IMunicipalityServiceRepository _municipalityRepo = municipalityRepo;
    private readonly IStateRepository _repository = repository;
    private readonly IElectionCycleRepository _cycleRepository = cycleRepository;

    public async Task Handle(FetchAndStoreStateMunicipalPartiesCommand request, CancellationToken cancellationToken)
    {
        var municipalityCodes = _municipalityRepo.GetAllMunicipalityCodes();

        await _repository.DeleteStateMunicipalPartiesAsync(request.Year);

        var cycle = await _cycleRepository.GetByYearAndTypeAsync(request.Year, ElectionType.GeneralElection);

        foreach (var municipalityCode in municipalityCodes)
        {
            var uri = $"{cycle.ApiBaseUrl}/race2_electoralunitpartyresult/{cycle.ResultKey}/{municipalityCode}/1";

            var stateMunicipalParties = await _centralCommissionClient
                .GetStateMunicipalPartiesAsync(uri)
                .ConfigureAwait(false);

            var entities = _mappingService.MapStateMunicipalParties(stateMunicipalParties, request.Year, municipalityCode);

            await _repository.StoreMunicipalPartiesAsync(entities).ConfigureAwait(false);
        }
    }
}
